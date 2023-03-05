using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using TMPro;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ObjectCountSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var turretCount = 0;
        foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Turret>())
        {
            turretCount++;
        }

        var bulletCount = 0;
        foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<CannonBall>())
        {
            bulletCount++;
        }

        var tankCount = 0;
        foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Tank>())
        {
            tankCount++;
        }

        var totalCount = turretCount + bulletCount + tankCount;

        var tmPro = UnitCounterSingleton.Instance;
        tmPro.text = "Object Counts: " + totalCount.ToString();
    }
}