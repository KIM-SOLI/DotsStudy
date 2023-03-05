using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public partial struct FollowEnemySystem : ISystem
{
    EntityQuery unitQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        unitQuery = SystemAPI.QueryBuilder().WithAll<TeamUnitComponentData>().Build();
        state.RequireForUpdate(unitQuery);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var delta = SystemAPI.Time.DeltaTime;
        var entities = unitQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        
        var job = new FollowEnemyJob
        {
            Delta = delta,
            entities = entities,
        };
        job.ScheduleParallel();

        //entities.Dispose();
    }
}



[BurstCompile]
partial struct FollowEnemyJob : IJobEntity
{
    [ReadOnly] public NativeArray<Entity> entities;
    [ReadOnly] public float Delta;

    void Execute(ref TeamUnitAspect value)
    {
        float3 targetPos = float3.zero;
        var maxSq = float.MaxValue;
        
        //manager.GetComponentData<WorldTransform>()
        //for (var i = 0; i < entities.Length; i++)
        //{
        //    var entity = entities[i];
        //    var entityPos = worldTransformLookup[entity].Position;

        //    var nextSq = math.distancesq(value.WorldPosition, entityPos);
        //    if (maxSq > nextSq)
        //    {
        //        maxSq = nextSq;
        //        targetPos = entityPos;
        //    }
        //}
        var dir = math.normalize(targetPos - value.WorldPosition) * Delta;

        value.WorldPosition += (dir);
    }
}