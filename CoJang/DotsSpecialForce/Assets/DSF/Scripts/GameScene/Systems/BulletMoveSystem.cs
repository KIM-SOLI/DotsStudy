using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
    //private EntityQuery bulletQuery;

    public void OnCreate(ref SystemState state)
    {
        //var bulletQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<BulletTag>();
        //bulletQuery = state.GetEntityQuery(bulletQueryBuilder);
        //state.RequireForUpdate(bulletQuery);

        //bulletQueryBuilder.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var bulletMovementJob = new BulletMovementJob
        {
            DeltaTime = deltaTime,

            ecb = ecb,
        };
        bulletMovementJob.Schedule();
    }
}

public partial struct BulletMovementJob : IJobEntity
{
    [ReadOnly] public float DeltaTime;
    public EntityCommandBuffer ecb;

    public void Execute(ref LocalTransform transform, in BulletTag bulletData)
    {
        transform.Position += transform.Forward() * (bulletData.BulletSpeed * DeltaTime);

        if (math.distancesq(transform.Position, bulletData.spawnedPosition) > bulletData.BulletRange)
        {
            if (bulletData.self != Entity.Null)
            {
                ecb.DestroyEntity(bulletData.self);
            }
        }
    }
}
