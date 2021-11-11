using Taichi.UNLang;
using Taichi.UNode;
using UnityEngine;

namespace Light
{
    public abstract class VehicleState : State
    {
        public const float AccelerateRatio = 4f;
        public const float DecelerateRatio = 6f;

        protected bool NearFollow()
        {
            var target = (IVehicle)this.Instance.Owner;
            if (target.Node.Previous == null)
            {
                return false;
            }

            if (target.Node.Previous.Value.IsOutOfView())
            {
                return false;
            }

            return Vector2.Distance(target.Position, target.Node.Previous.Value.Position) < 3f;
        }
    }

    [NodeInterface("启动汽车", "红绿灯/")]
    public sealed class StartupState : VehicleState
    {
        public override void Init()
        {
            base.Init();

            Add(new LangSpot("进入平稳驾驶", LangType.Category.Any, this, -1, SpotType.Out));
            Add(new LangSpot("触发刹车", LangType.Category.Any, this, -1, SpotType.Out));
        }

        protected override void OnExecute()
        {
            var target = (IVehicle)this.Instance.Owner;
            target.Speed += AccelerateRatio * Time.deltaTime;
            target.Position += Time.deltaTime * target.Direction * target.Speed;

            if (Mathf.Abs(target.Speed - target.MaxSpeed) < 0.01f || target.Speed >= target.MaxSpeed)
            {
                GetByName("进入平稳驾驶").Signal(this.Instance);
                return;
            }

            if (!target.Auto)
            {
                return;
            }

            if (!target.IsMovable() || NearFollow())
            {
                GetByName("触发刹车").Signal(this.Instance);
            }
        }
    }

    [NodeInterface("平稳驾驶", "红绿灯/")]
    public sealed class DriveState : VehicleState
    {
        public override void Init()
        {
            base.Init();

            Add(new LangSpot("触发刹车", LangType.Category.Any, this, -1, SpotType.Out));
        }

        protected override void OnEnter()
        {
            var target = (IVehicle)this.Instance.Owner;
            target.Speed = target.MaxSpeed;
        }

        protected override void OnExecute()
        {
            var target = (IVehicle)this.Instance.Owner;
            target.Position += Time.deltaTime * target.Direction * target.Speed;

            if (!target.Auto)
            {
                return;
            }

            if (!target.IsMovable() || NearFollow())
            {
                GetByName("触发刹车").Signal(this.Instance);
            }
        }
    }

    [NodeInterface("刹车", "红绿灯/")]
    public sealed class BrakeState : VehicleState
    {
        private Vector2 stopLine = Vector2.zero;

        public override void Init()
        {
            base.Init();

            Add(new LangSpot("停车", LangType.Category.Any, this, -1, SpotType.Out));
            Add(new LangSpot("启动汽车", LangType.Category.Any, this, -1, SpotType.Out));
        }

        protected override void OnEnter()
        {
            var target = (IVehicle)this.Instance.Owner;
            var lightStopLine = target.Light.StopLine;
            this.stopLine = new Vector2
            (
                lightStopLine.x == -1 ? target.Position.x : lightStopLine.x,
                lightStopLine.y == -1 ? target.Position.y : lightStopLine.y
            );
        }

        protected override void OnExecute()
        {
            var target = (IVehicle)this.Instance.Owner;
            if (Vector2.Distance(target.Position, this.stopLine) < 3f ||
                (target.Node.Previous != null && Vector2.Distance(target.Position, target.Node.Previous.Value.Position) < 8f))
            {
                target.Speed -= DecelerateRatio * Time.deltaTime;
            }
            else if (target.Speed < target.MaxSpeed)
            {
                target.Speed += AccelerateRatio * Time.deltaTime;
            }
            else
            {
                target.Speed = target.MaxSpeed;
            }

            target.Position += Time.deltaTime * target.Direction * target.Speed;

            if (NearStopLine() || NearFollow() || target.Speed < 0.05f)
            {
                GetByName("停车").Signal(this.Instance);
            }
            else if (target.Auto && target.IsMovable())
            {
                GetByName("启动汽车").Signal(this.Instance);
            }
        }

        private bool NearStopLine()
        {
            var target = (IVehicle)this.Instance.Owner;
            return Vector2.Distance(target.Position, this.stopLine) < 2f;
        }
    }

    [NodeInterface("停车", "红绿灯/")]
    public sealed class StopState : VehicleState
    {
        public override void Init()
        {
            base.Init();

            Add(new LangSpot("启动汽车", LangType.Category.Any, this, -1, SpotType.Out));
        }

        protected override void OnEnter()
        {
            var target = (IVehicle)this.Instance.Owner;
            target.Speed = 0f;
        }

        protected override void OnExecute()
        {
            var target = (IVehicle)this.Instance.Owner;
            if (target.IsMovable())
            {
                GetByName("启动汽车").Signal(this.Instance);
            }
        }
    }

    [NodeInterface("违章", "红绿灯/")]
    public sealed class BreakRuleState : VehicleState
    {
        private const float ThrowInitialSpeed = 30f;
        private const float ThrowSpeedRatio = 10f;
        private const float Interval = 2f;
        private static readonly Vector2 ThrowDir = new Vector2(1, 1);

        private float throwSpeed = 0f;
        private float timer = 0;

        protected override void OnEnter()
        {
            this.throwSpeed = ThrowInitialSpeed;
            this.timer = 0;
        }

        protected override void OnExecute()
        {
            this.throwSpeed -= Time.deltaTime * ThrowSpeedRatio;

            var target = (IVehicle)this.Instance.Owner;
            target.Position += Time.deltaTime * ThrowDir * this.throwSpeed;

            this.timer += Time.deltaTime;
            if (this.timer >= Interval)
            {
                target.Dispose();
            }
        }
    }
}
