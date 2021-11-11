# Light

红绿灯游戏。

## 玩法

* 横屏
* 小汽车以不同速度从左向右驶过屏幕
* 行人从上向下走过人行道
* 通过点击汽车和行人，控制汽车和行人走/停
* 屏幕顶部一个醒目的红绿灯，每隔一段时间切换一种颜色，每种灯有倒计时
* 在倒计时内，玩家需要根据灯的颜色，点击汽车和行人，控制他们走或者停
* 红灯内，车没有停止会闯红灯或追尾，游戏失败；行人没有停止会闯红灯，游戏失败。绿灯内，车没有开走或者启动顺序错误，游戏失败；行人没有过马路，游戏失败。

## 技术方案

场景内有一条马路和一条人行道，马路为N车道。机动车仅展现单向车流；人行道展现单向行人。场景内有两个红绿灯，一个控制机动车，一个控制行人。两个红绿灯有M秒时间差。每个红绿灯可以知道启停线，在红灯的情况下过线即为闯红灯。

### ICity

城市接口。管理在街道上的行人，车辆和红绿灯。

场景以左下角为（0，0）点，x轴水平向右，y轴垂直向上。
机动车道长：30米，宽10米，启停线为（25，y）
人行道长：10米，启停线（x，30）


```csharp
public interface ICity
{
    void Start();
    void Stop();
    void Update(float deltaTime);
}
```

### City

城市对象。

```csharp
public class City : ICity
{
    private ITraficLight vehicleLight = null;
    private ITraficLight pedestrianLight = null;
    private readonly List<IMovableObjectEmitter> movableObjectEmitters = new List<IMovableObjectEmitter>();
    private readonly LinkedList<IMovableObject> movableObjects = new LinkedList<IMovableObject>();

    public City()
    { }

    public void Update(float deltaTime)
    {
        this.vehicleLight.Update(deltaTime);
        this.pedestrianLight.Update(deltaTime);

        for (var i = 0; i < this.movableObjectEmitters.Count; ++i)
        {
            this.movableObjectEmitters[i].Update(deltaTime);
        }

        // 注意更新过程中的销毁问题。
        var n = this.movableObjects.First();
        while (n != null)
        {
            n.Value.Update(deltaTime);
            n = n.Next;
        }
    }
}
```

### ITraficLight

交通灯接口。

```csharp
public interface ITraficLight
{
    Vector2 StopLine { get; }
    bool IsRed { get; }

    void Update(float deltaTime);
}
```

### TraficLight

交通灯对象。

```csharp
public class TraficLight : ITraficLight
{
    // 初始化红灯，黄灯和绿灯时长，停止线位置，以及是否以红灯开始（反之，以绿灯开始）
    public TraficLight(float red, float yellow, float green, Vector2 stopLine, bool redAsStart)
    { }
}
```

### IMovableObject

场景中所有的可移动对象接口。可以控制启动和停止；可以查询是否失败：车辆追尾，车辆或行人闯红灯，车辆未按时启动，行人未及时过马路等。

```csharp
public interface IMovableObject
{
    bool IsFailed { get; }

    void Start();
    void Stop();

    void Update(float deltaTime);
}
```

### MovableObject

场景中可移动物体的抽象基类。

```csharp
public abstract class MovableObject : IMovableObject
{
    // 初始化起点和速度。
    public MovableObject(Vector2 origin, Vector2 velocity, ITraficLight light)
    {
    }
}
```

### Vehicle

机动车对象。创建后从起点以一定的速度移动，每一帧要判断是否闯红灯，是否追尾。机动车启动和停止需要有加速度。正常驶出屏幕则自动销毁。

```csharp
public class Vehicle : MovableObject
{
}
```

### Pedestrian

行人对象。创建后从起点以一定速度移动，每一帧判断是否闯红灯。行人启动和停止没有加速度。正常走出屏幕则自动销毁。

```csharp
public class Pedestrian : MovableObjet
{
}
```

### IMovableObjectEmitter

可移动对象发射器，用来产生特定的可移动物体。

```csharp
public interface IMovableObjectEmitter
{
    void Update(float deltaTime);
}
```

### VehicleEmitter

机动车发射器。每隔一定时间在固定位置产生一辆机动车。

```csharp
public class VehicleEmitter : IMovableObjectEmitter
{
    public VehicleEmitter(Vector2 origin, Vector2 velocity, float interval)
    { }
}
```

### PedestrianEmitter

行人发射器。每隔一定时间在固定位置（或随机位置）产生一个行人。

```csharp
public class PedestrianEmitter : IMovableObjectEmitter
{
    public PedestrianEmitter(Vector2 origin, Vector2 velocity, float interval)
    { }
}
```

