using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySpawnSystem : ISystem
{
    private EntityQuery query;
    private float timer;
    private float totalTime; // 시드 랜덤으로 뽑으려고..

    private Entity spawn;
    private float spawnRate;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var playerGetQueryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<Spawn>();
        query = state.GetEntityQuery(playerGetQueryBuilder);

        state.RequireForUpdate(query);

        timer = 0;
        totalTime = 0;
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
            spawn = SystemAPI.GetSingletonEntity<Spawn>();
            spawnRate = SystemAPI.GetComponent<Spawn>(spawn).spawnRate;
        }

        totalTime += SystemAPI.Time.DeltaTime;
        timer += SystemAPI.Time.DeltaTime;
        if (timer < spawnRate)
        {
            return;
        }

        timer = 0f;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var random = Random.CreateFromIndex((uint)totalTime);

        var job = new EnemySpawnJob
        {
            spawnHandle = state.GetComponentTypeHandle<Spawn>(true),
            enemyCountHandle = state.GetComponentTypeHandle<EnemyCount>(true),
            random = random,
            entityHandle = state.GetEntityTypeHandle(),
            writer = ecb.AsParallelWriter(),
        };

        state.Dependency = job.ScheduleParallel(query, state.Dependency);
    }
}

public partial struct EnemySpawnJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<Spawn> spawnHandle;
    [ReadOnly] public ComponentTypeHandle<EnemyCount> enemyCountHandle;
    [ReadOnly] public Random random;

    public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter writer;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var spawns = chunk.GetNativeArray(ref spawnHandle);
        var enemyCounts = chunk.GetNativeArray(ref enemyCountHandle);

        var enumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
        while (enumerator.NextEntityIndex(out var index))
        {
            var count = enemyCounts[index];
            for (uint i = 0; i < spawns[index].spawnCount; i++)
            {
                // 적 카운트 세기
                count.enemyCount += 1;
                writer.SetComponent(index, entities[index], count);

                // 적 생성
                writer.Instantiate(index, spawns[index].enemyPrefab);
                writer.SetComponent(index, spawns[index].enemyPrefab, new LocalTransform
                {
                    Position = new float3(random.NextFloat(-100f, 100f), 1.5f, random.NextFloat(-100f, 100f)),
                    Rotation = quaternion.identity,
                    Scale = 1,
                });
                writer.SetComponent(index, spawns[index].enemyPrefab, new EnemyTag());
                writer.SetComponent(index, spawns[index].enemyPrefab, new DefaltCharacterComponent
                {
                    Speed = 3,
                    HP = 100,
                });
            }
        }
    }
}