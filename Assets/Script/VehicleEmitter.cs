using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Light
{
    public sealed class VehicleEmitter : IMovableObjectEmitter
    {
        private readonly ITrafficLight light = null;
        private readonly float minInterval = 0f;
        private readonly float maxInterval = 0f;
        private readonly Func<IMovableObject> generator = null;
        private readonly LinkedList<IMovableObject> movableObjects = new LinkedList<IMovableObject>();
        private float timer = 0;

        public VehicleEmitter(ITrafficLight light, float minInterval, float maxInterval, Func<IMovableObject> generator)
        {
            this.light = light;
            this.minInterval = minInterval;
            this.maxInterval = maxInterval;
            this.generator = generator;
            this.timer = NextInterval();

            EmitMovableObject();
        }

        public int BadObjectCount
        {
            get
            {
                int sum = 0;
                var n = this.movableObjects.First;
                while (n != null)
                {
                    if (!n.Value.Auto)
                    {
                        ++sum;
                    }

                    n = n.Next;
                }

                return sum;
            }
        }

        public void Update(float deltaTime)
        {
            var n = this.movableObjects.Last;
            while (n != null)
            {
                n.Value.Update(deltaTime);
                if (n.Value.IsBroken)
                {
                    n.Value.Broken();
                }

                n = n.Previous;
            }

            if (this.light.IsRed)
            {
                return;
            }

            deltaTime *= this.light.IsYellow ? 0.5f : 1f;

            this.timer -= deltaTime;
            if (this.timer <= 0)
            {
                this.timer = NextInterval();
                EmitMovableObject();
            }
        }

        public IMovableObject HitTest(Vector2 v)
        {
            var n = this.movableObjects.First;
            while (n != null)
            {
                if (!n.Value.Auto && n.Value.HitTest(v))
                {
                    return n.Value;
                }

                n = n.Next;
            }

            return null;
        }

        private float NextInterval()
        {
            return this.minInterval + UnityEngine.Random.Range(0, this.maxInterval - this.minInterval);
        }

        private void EmitMovableObject()
        {
            var obj = this.generator?.Invoke();
            obj.OnDestroy += OnMovableObjectDestroy;
            obj.Node = this.movableObjects.AddLast(obj);
        }

        private void OnMovableObjectDestroy(IMovableObject obj)
        {
            this.movableObjects.Remove(obj.Node);
        }
    }
}
