using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

[RequireMatchingQueriesForUpdate]
public partial struct BulletSpawnSystem : ISystem
{
    private EntityQuery query;
    private float timer;

    private Entity spawn;
    private float spawnRate;
    private Vector3 spawnPos;
    private Entity player;
    private Vector3 playerPos;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var playerGetQueryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<BulletSpawnTag>();
        query = state.GetEntityQuery(playerGetQueryBuilder);

        state.RequireForUpdate(query);

        timer = 0;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (spawnRate == 0)
        {
            spawn = SystemAPI.GetSingletonEntity<BulletSpawnTag>();
            spawnRate = SystemAPI.GetComponent<Spawn>(spawn).spawnRate;
            spawnPos = SystemAPI.GetComponent<LocalTransform>(spawn).Position;
        }

        timer += SystemAPI.Time.DeltaTime;
        if (timer < spawnRate)
        {
            return;
        }

        timer = 0f;

        player = SystemAPI.GetSingletonEntity<PlayerTag>();
        playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new BulletSpawnJob
        {
            LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
            LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            spawnHandle = state.GetComponentTypeHandle<Spawn>(true),
            spawnPosition = spawnPos + playerPos,

            entityHandle = state.GetEntityTypeHandle(),
            writer = ecb.AsParallelWriter(),
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }
}

public partial struct BulletSpawnJob : IJobChunk
{
    [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
    [ReadOnly] public ComponentTypeHandle<Spawn> spawnHandle;

    [ReadOnly] public float3 spawnPosition;

    public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter writer;

    //public NativeArray<Spawn> spawns;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var spawns = chunk.GetNativeArray(ref spawnHandle);

        var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
        while (enumerator.NextEntityIndex(out var index))
        {
            var entity = entities[index];

            var crateUnits = CollectionHelper.CreateNativeArray<Entity>(spawns[index].spawnCount, Allocator.Temp);
            writer.Instantiate(unfilteredChunkIndex, spawns[index].prefab, crateUnits);

            var spawnLocalToWorld = LocalToWorldLookup[entity];

            for (int i = 0; i < spawns[index].spawnCount; i++)
            {
                // ÃÑ¾Ë »ý¼º
                writer.Instantiate(unfilteredChunkIndex, spawns[index].prefab, crateUnits);
                writer.SetComponent(unfilteredChunkIndex, spawns[index].prefab, new LocalTransform
                {
                    Position = spawnLocalToWorld.Position,
                    Rotation = quaternion.identity,
                    Scale = LocalTransformLookup[spawns[index].prefab].Scale,
                });
                writer.SetComponent(unfilteredChunkIndex, spawns[index].prefab, new Bullet
                {
                    Speed = spawnLocalToWorld.Forward * 20.0f,
                });
            }
        }
    }
}