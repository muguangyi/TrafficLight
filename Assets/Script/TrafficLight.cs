using System;
using Taichi.Asset;
using Taichi.Foundation;
using Taichi.Logger;
using UnityEngine;

namespace Light
{
    public sealed class TrafficLight : ITrafficLight
    {
        private class TrafficState : ITrafficState
        {
            private readonly float interval = 0;
            private ITrafficState nextState = null;
            private float timer = 0;

            public TrafficState(LightType type, float interval)
            {
                this.Type = type;
                this.interval = interval;
            }

            public LightType Type { get; } = LightType.Red;

            public void Enter(ITrafficLight light)
            {
                this.timer = this.interval;
            }

            public void Execute(ITrafficLight light, float deltaTime)
            {
                this.timer -= deltaTime;
                if (this.timer <= 0)
                {
                    light.ChangeState(this.nextState);
                }
            }

            public void Exit(ITrafficLight light)
            { }

            public ITrafficState Next(ITrafficState nextState)
            {
                this.nextState = nextState;
                return nextState;
            }
        }

        [Resolve] private static IGObjectFactory factory = null;

        private readonly float red = 0;
        private readonly float yellow = 0;
        private readonly float green = 0;

        private ITrafficState state = null;

        public TrafficLight(float red, float yellow, float green, Vector2 stopLine, bool redAsStart)
        {
            this.red = red;
            this.yellow = yellow;
            this.green = green;
            this.StopLine = stopLine;

            var redState = new TrafficState(LightType.Red, red);
            var greenState = new TrafficState(LightType.Green, green);
            redState.Next(new TrafficState(LightType.Yellow, yellow))
                    .Next(greenState)
                    .Next(new TrafficState(LightType.Yellow, yellow))
                    .Next(redState);

            ChangeState(redAsStart ? redState : greenState);
        }

        public event Action<ITrafficState> OnLightStateEnter;
        public event Action<ITrafficState> OnLightStateExit;

        public Vector2 StopLine { get; }

        public bool IsRed => this.state != null ? this.state.Type == LightType.Red : false;

        public bool IsYellow => this.state != null ? this.state.Type == LightType.Yellow : false;

        public bool IsGreen => this.state != null ? this.state.Type == LightType.Green : false;

        public void Update(float deltaTime)
        {
            this.state?.Execute(this, deltaTime);
        }

        public void ChangeState(ITrafficState state)
        {
            if (this.state != null)
            {
                this.OnLightStateExit?.Invoke(this.state);
                this.state.Exit(this);
            }

            this.state = state;

            if (this.state != null)
            {
                this.state?.Enter(this);
                this.OnLightStateEnter?.Invoke(this.state);
            }
        }
    }
}
