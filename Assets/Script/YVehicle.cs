using UnityEngine;

namespace Light
{
    public sealed class YVehicle : Vehicle
    {
        public YVehicle(Vector2 origin, float maxSpeed, ITrafficLight light, bool auto = true)
            : base(PickCarAsset(), origin, Vector2.down, maxSpeed, light, auto)
        { }

        public override bool IsBroken
        {
            get
            {
                if (this.Auto || !IsInView())
                {
                    return false;
                }

                if (this.Light.IsRed && Mathf.Abs(this.Light.StopLine.y - this.Position.y) < VehicleSize.y * 0.5f)
                {
                    return true;
                }

                if (this.Node.Previous != null && Mathf.Abs(this.Node.Previous.Value.Position.y - this.Position.y) < VehicleSize.y)
                {
                    return true;
                }

                if (this.Node.Next != null && Mathf.Abs(this.Node.Next.Value.Position.y - this.Position.y) < VehicleSize.y)
                {
                    return true;
                }

                return false;
            }
        }

        public override bool IsInView()
        {
            return this.Position.y < 15f;
        }

        public override bool IsOutOfView()
        {
            return this.Position.y < -15f;
        }

        public override bool IsMovable()
        {
            if (this.Light.IsGreen)
            {
                return true;
            }

            return this.Position.y < this.Light.StopLine.y;
        }

        private static string PickCarAsset()
        {
            var v = Random.value;
            if (v < 0.3f)
            {
                return "Element/WhiteLorry";
            }
            else if (v < 0.7f)
            {
                return "Element/OrangeLorry";
            }
            else
            {
                return "Element/BlueLorry";
            }
        }
    }
}
