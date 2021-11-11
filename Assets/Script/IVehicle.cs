using UnityEngine;

namespace Light
{
    public interface IVehicle : IMovableObject
    {
        Vector2 Direction { get; }
        float MaxSpeed { get; }
        float Speed { get; set; }

        bool IsMovable();
    }
}
