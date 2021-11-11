using UnityEngine;

namespace Light
{
    public interface IMovableObjectEmitter
    {
        int BadObjectCount { get; }

        void Update(float deltaTime);
        IMovableObject HitTest(Vector2 v);
    }
}
