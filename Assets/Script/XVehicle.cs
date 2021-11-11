using UnityEngine;

namespace Light
{
    public sealed class XVehicle : Vehicle
    {
        public XVehicle(Vector2 origin, float maxSpeed, ITrafficLight light, bool auto = true)
            : base(PickCarAsset(auto), origin, Vector2.right, maxSpeed, light, auto)
        { }

        public override bool IsBroken
        {
            get
            {
                if (this.Auto || !IsInView())
                {
                    return false;
                }

                if (this.Light.IsRed && Mathf.Abs(this.Light.StopLine.x - this.Position.x) < VehicleSize.x * 0.5f)
                {
                    return true;
                }

                if (this.Node.Previous != null && Mathf.Abs(this.Node.Previous.Value.Position.x - this.Position.x) < VehicleSize.x)
                {
                    return true;
                }

                if (this.Node.Next != null && Mathf.Abs(this.Node.Next.Value.Position.x - this.Position.x) < VehicleSize.x)
                {
                    return true;
                }

                return false;
            }
        }

        public override bool IsInView()
        {
            return this.Position.x > -20f;
        }

        public override bool IsOutOfView()
        {
            return this.Position.x > 25f;
        }

        public override bool IsMovable()
        {
            if (this.Light.IsGreen)
            {
                return true;
            }

            return this.Position.x > this.Light.StopLine.x;
        }

        private static string PickCarAsset(bool auto)
        {
            if (auto)
            {
                return Random.value > 0.5f ? "Element/BlueCar" : "Element/BlueTruck";
            }
            else
            {
                return Random.value > 0.5f ? "Element/RedCar" : "Element/RedTruck";
            }
        }
    }
}
