using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial struct BulletMoveSystem : ISystem
{
    EntityQuery query;

    private Entity player;
    private Vector3 playerPos;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var playerGetQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<Bullet>();
        query = state.GetEntityQuery(playerGetQueryBuilder);

        state.RequireForUpdate(query);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        player = SystemAPI.GetSingletonEntity<PlayerTag>();
        playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        var job = new BulletMoveJob
        {
            myPositions = state.GetComponentTypeHandle<LocalTransform>(true),
            bulletHandles = state.GetComponentTypeHandle<Bullet>(true),

            playerPos = playerPos,
            deltaTime = SystemAPI.Time.DeltaTime,

            entityHandle = state.GetEntityTypeHandle(),
            writer = ecb.AsParallelWriter(),
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }
}

public partial struct BulletMoveJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<LocalTransform> myPositions;
    [ReadOnly] public ComponentTypeHandle<Bullet> bulletHandles;

    [ReadOnly] public float3 playerPos;
    [ReadOnly] public float deltaTime;

    public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter writer;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var localToWorlds = chunk.GetNativeArray(myPositions);
        var bullets = chunk.GetNativeArray(ref bulletHandles);

        var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
        while (enumerator.NextEntityIndex(out var index))
        {
            var entity = entities[index];
            var localTransform = localToWorlds[index];

            var gravity = new float3(0.0f, -9.82f, 0.0f);

            localTransform.Position += bullets[index].Speed * deltaTime;
            float3 velocity = bullets[index].Speed + gravity * deltaTime * 0.5f;

            writer.SetComponent(unfilteredChunkIndex, entity, localTransform);
            writer.SetComponent(unfilteredChunkIndex, entity, new Bullet { Speed = velocity });

            var speed = math.lengthsq(bullets[index].Speed);
            if (speed < 0.1f)
            {
                writer.DestroyEntity(unfilteredChunkIndex, entity);
            }
        }
    }
}