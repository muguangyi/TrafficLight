namespace Light
{
    public enum LightType
    {
        Red,
        Yellow,
        Green,
    }

    public interface ITrafficState
    {
        LightType Type { get; }

        void Enter(ITrafficLight light);
        void Exit(ITrafficLight light);
        void Execute(ITrafficLight light, float deltaTime);
        ITrafficState Next(ITrafficState nextState);
    }
}
