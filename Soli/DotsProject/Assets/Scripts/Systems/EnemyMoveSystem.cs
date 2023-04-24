using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemyMoveSystem : ISystem
{
    EntityQuery query;

    private Entity player;
    private Vector3 targetPos;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var playerGetQueryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<EnemyTag>();
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
        targetPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        var job = new EnemyMoveJob
        {
            myPositions = state.GetComponentTypeHandle<LocalTransform>(true),
            characterHandles = state.GetComponentTypeHandle<DefaltCharacterComponent>(true),

            targetPosition = targetPos,
            deltaTime = SystemAPI.Time.DeltaTime,

            entityHandle = state.GetEntityTypeHandle(),
            writer = ecb.AsParallelWriter(),
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }
}

public partial struct EnemyMoveJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<LocalTransform> myPositions;
    [ReadOnly] public ComponentTypeHandle<DefaltCharacterComponent> characterHandles;

    [ReadOnly] public float3 targetPosition;
    [ReadOnly] public float deltaTime;

    public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter writer;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var localToWorlds = chunk.GetNativeArray(myPositions);
        var characters = chunk.GetNativeArray(ref characterHandles);

        var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
        while (enumerator.NextEntityIndex(out var index))
        {
            var entity = entities[index];
            var localTransform = localToWorlds[index];

            Vector3 dir = targetPosition - localTransform.Position;
            localTransform.Position += deltaTime * (float3)dir.normalized * characters[index].Speed;

            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = new Vector3(0, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, 0);
            localTransform.Rotation = rot;

            writer.SetComponent(unfilteredChunkIndex, entity, localTransform);
        }
    }
}