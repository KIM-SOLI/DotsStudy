using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
    EntityQuery entityQuery;
    public void OnCreate(ref SystemState state)
    {
        var entityQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<BulletTag>();
        entityQuery = state.GetEntityQuery(entityQueryBuilder);
        state.RequireForUpdate(entityQuery);
        entityQueryBuilder.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var entityArray = entityQuery.ToEntityArray(Allocator.TempJob);

        var bulletMovementJob = new BulletMovementJob
        {
            deltaTime = deltaTime,

            ecb = ecb.AsParallelWriter(),
            entities = entityArray,
        };
        bulletMovementJob.ScheduleParallel();
    }
}

public partial struct BulletMovementJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    [ReadOnly] public NativeArray<Entity> entities;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(ref LocalTransform transform, in BulletTag bulletData, in DestoryTag tag)
    {
        if (bulletData.self != Entity.Null && !tag.IsDestoryed)
        {
            transform.Position += transform.Forward() * (bulletData.BulletSpeed * deltaTime);

            if (bulletData.PanetrateNum <= 0 ||
                (math.distancesq(transform.Position, bulletData.spawnedPosition) > bulletData.BulletRange))
            {
                var tempData = tag;
                tempData.IsDestoryed = true;

                ecb.SetComponent(bulletData.self.Index, bulletData.self, tempData);
            }
        }
    }
}
