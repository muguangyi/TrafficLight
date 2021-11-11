using System;
using System.Collections.Generic;
using UnityEngine;

namespace Light
{
    public interface IMovableObject : IDisposable
    {
        event Action<IMovableObject> OnDestroy;

        bool IsBroken { get; }
        Vector2 Position { get; set; }
        ITrafficLight Light { get; }
        LinkedListNode<IMovableObject> Node { get; set; }
        bool Auto { get; }

        bool IsInView();
        bool IsOutOfView();

        void Click();
        void Broken();

        void Update(float deltaTime);
        bool HitTest(Vector2 v);
    }
}
