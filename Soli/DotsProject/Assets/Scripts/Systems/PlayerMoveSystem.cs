using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UIElements;
using Unity.Burst.Intrinsics;
using Unity.Transforms;
using Unity.Mathematics;
using static Unity.Entities.Content.RemoteContentLocation;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SocialPlatforms;

[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
{
    Vector3 inputVec;
    EntityQuery query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var playerGetQueryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerTag>();
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
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");
        inputVec = inputVec.normalized;

        if (inputVec is { x: 0, z: 0 })
        {
            return;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new PlayerMoveJob
        {
            myPositions = state.GetComponentTypeHandle<LocalTransform>(true),
            characterHandles = state.GetComponentTypeHandle<DefaltCharacterComponent>(true),
            deltaTime = SystemAPI.Time.DeltaTime,
            moveFloat3 = inputVec,

            entityHandle = state.GetEntityTypeHandle(),
            writer = ecb.AsParallelWriter(),
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }
}

public partial struct PlayerMoveJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<LocalTransform> myPositions;
    [ReadOnly] public ComponentTypeHandle<DefaltCharacterComponent> characterHandles;

    [ReadOnly] public float deltaTime;
    [ReadOnly] public Vector3 moveFloat3;

    public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter writer;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var localTransforms = chunk.GetNativeArray(myPositions);
        var characters = chunk.GetNativeArray(ref characterHandles);

        var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
        while (enumerator.NextEntityIndex(out var index))
        {
            var entity = entities[index];
            var localTransform = localTransforms[index];

            float3 dir = moveFloat3.normalized;
            localTransform.Position += deltaTime * dir * characters[index].Speed;

            writer.SetComponent(unfilteredChunkIndex, entity, localTransform);
        }
    }
}
