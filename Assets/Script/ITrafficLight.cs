using System;
using UnityEngine;

namespace Light
{
    public interface ITrafficLight
    {
        event Action<ITrafficState> OnLightStateEnter;
        event Action<ITrafficState> OnLightStateExit;

        Vector2 StopLine { get; }
        bool IsRed { get; }
        bool IsYellow { get; }
        bool IsGreen { get; }

        void Update(float deltaTime);
        void ChangeState(ITrafficState state);
    }
}
