using UnityEngine;

namespace Light
{
    public sealed class VehicleStats : MonoBehaviour
    {
        private IVehicle vehicle = null;

        public void Bind(IVehicle vehicle)
        {
            this.vehicle = vehicle;
        }

        public void OnDrawGizmos()
        {
            if (this.vehicle != null && this.vehicle.Node.Previous != null)
            {
                var start = new Vector3(this.vehicle.Position.x, this.vehicle.Position.y);
                var end = new Vector3(this.vehicle.Node.Previous.Value.Position.x, this.vehicle.Node.Previous.Value.Position.y);
                Debug.DrawLine(start, end, Color.red);
            }
        }
    }
}
