using System;
using System.Collections.Generic;
using Taichi.Asset;
using Taichi.Foundation;
using Taichi.UNLang;
using Taichi.UNode;
using UnityEngine;

namespace Light
{
    public class Vehicle : IVehicle
    {
        [Resolve] private static IAssetFactory f = null;
        [Resolve] private static IGObjectFactory factory = null;

        private static readonly Dictionary<string, Stack<GameObject>> viewCache = new Dictionary<string, Stack<GameObject>>();
        public static readonly Vector2 VehicleSize = new Vector2(3, 3);

        private LangInstance lang = null;
        private readonly string asset = string.Empty;
        private Vector2 position = default;
        private Vector3 viewPosition = new Vector3();
        private GameObject view = null;

        public Vehicle(string asset, Vector2 origin, Vector2 direction, float maxSpeed, ITrafficLight light, bool auto)
        {
            this.asset = asset;
            this.Direction = direction.normalized;
            this.MaxSpeed = maxSpeed;
            this.Light = light;
            this.Auto = auto;

            this.position = origin;
            this.viewPosition.Set(this.position.x, this.position.y, 0f);

            this.view = PickView(this.asset);
            this.view.transform.SetPositionAndRotation(this.viewPosition, Quaternion.identity);
            var stats = this.view.GetComponent<VehicleStats>();
            stats.Bind(this);

            this.lang = new LangInstance(this, new LangVars());
            this.lang.Load("UNLang/vehicle");
            this.lang.Run<StartupState>();
        }

        public event Action<IMovableObject> OnDestroy;

        public virtual bool IsBroken => false;

        public Vector2 Position
        {
            get => this.position;

            set
            {
                this.position = value;
                this.viewPosition.Set(value.x, value.y, 0);
                this.view.transform.position = this.viewPosition;
            }
        }

        public float MaxSpeed { get; } = 0f;

        public float Speed { get; set; } = 0f;

        public Vector2 Direction { get; } = Vector2.right;

        public bool Auto { get; } = true;

        public ITrafficLight Light { get; } = null;

        public LinkedListNode<IMovableObject> Node { get; set; } = null;

        public virtual void Dispose()
        {
            this.OnDestroy?.Invoke(this);

            var stats = this.view.GetComponent<VehicleStats>();
            stats.Bind(null);

            DropView(this.asset, this.view);
        }

        public virtual bool IsInView()
        {
            return false;
        }

        public virtual bool IsOutOfView()
        {
            return false;
        }

        public virtual bool IsMovable()
        {
            return true;
        }

        public void Click()
        {
            this.lang?.Run<BrakeState>();
        }

        public void Broken()
        {
            this.lang?.Run<BreakRuleState>();
        }

        public void Update(float deltaTime)
        {
            this.lang?.Update();

            if (IsOutOfView())
            {
                Dispose();
            }
        }

        public bool HitTest(Vector2 v)
        {
            return v.x > (this.position.x - VehicleSize.x * 0.5f) &&
                   v.x < (this.position.x + VehicleSize.x * 0.5f) &&
                   v.y > (this.position.y - VehicleSize.y * 0.5f) &&
                   v.y < (this.position.y + VehicleSize.y * 0.5f);
        }

        private static GameObject PickView(string asset)
        {
            if (!viewCache.TryGetValue(asset, out Stack<GameObject> pool))
            {
                viewCache.Add(asset, pool = new Stack<GameObject>());
            }

            if (pool.Count > 0)
            {
                var obj = pool.Pop();
                obj.SetActive(true);
                return obj;
            }

            return factory.Instantiate(asset);
        }

        private static void DropView(string asset, GameObject go)
        {
            if (viewCache.TryGetValue(asset, out Stack<GameObject> pool))
            {
                pool.Push(go);
                go.SetActive(false);
            }
        }
    }
}
