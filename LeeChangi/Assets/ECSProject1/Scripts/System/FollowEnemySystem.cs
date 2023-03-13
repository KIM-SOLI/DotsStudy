using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct FollowEnemySystem : ISystem
{
    EntityQuery unitQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<TeamUnitComponentData>()
            .WithAllRW<LocalToWorld>();
        unitQuery = state.GetEntityQuery(unitQueryBuilder);
        state.RequireForUpdate(unitQuery);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var unitCount = unitQuery.CalculateEntityCount();
        var world = state.WorldUnmanaged;
        var delta = SystemAPI.Time.DeltaTime;


        var copyUnitPositions = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(unitCount,ref world.UpdateAllocator);
        var copyUnitIds = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
        var unitChunkBaseEntityIndexArray = unitQuery.CalculateBaseEntityIndexArrayAsync(
            world.UpdateAllocator.ToAllocator, state.Dependency,
            out var unitChunkBaseIndexJobHandle);

        var initialUnitJob = new InitialPerUnitJob
        {
            chunkBaseEntitiyIndices = unitChunkBaseEntityIndexArray,
            unitPositions = copyUnitPositions,
            unitIds = copyUnitIds,
        };

        var jobHandle = initialUnitJob.ScheduleParallel(unitQuery, unitChunkBaseIndexJobHandle);


        var job = new FollowEnemyJob
        {
            Delta = delta,
            unitIds = copyUnitIds,
            unitPositions = copyUnitPositions,
            //entities = entities,
        };
        var handle = job.Schedule(jobHandle);
        state.Dependency = handle;
        //entities.Dispose();
    }
}



[BurstCompile]
partial struct FollowEnemyJob : IJobEntity
{
    
    [ReadOnly] public NativeArray<float3> unitPositions;
    [ReadOnly] public NativeArray<int> unitIds;
    [ReadOnly] public float Delta;

    void Execute(ref TeamUnitAspect value)
    {
        float3 targetPos = float3.zero;
        var maxSq = float.MaxValue;

        for (var i = 0; i < unitIds.Length; i++)
        {
            var entity = unitIds[i];
            if (entity == value.TeamIndex)
            {
                continue;
            }
            var entityPos = unitPositions[i];

            var nextSq = math.distancesq(value.WorldPosition, entityPos);
            if (maxSq > nextSq)
            {
                maxSq = nextSq;
                targetPos = entityPos;
            }
        }
        var dir = math.normalize(targetPos - value.WorldPosition) * Delta;

        value.WorldPosition += (dir);
    }
}


partial struct InitialPerUnitJob : IJobEntity
{
    [ReadOnly] public NativeArray<int> chunkBaseEntitiyIndices;
    [NativeDisableParallelForRestriction] public NativeArray<float3> unitPositions;
    [NativeDisableParallelForRestriction] public NativeArray<int> unitIds;

    void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk,
        in LocalToWorld localToWorld, in TeamUnitAspect entity)
    {
        var entityIndexInQuery = chunkBaseEntitiyIndices[chunkIndexInQuery] + entityIndexInChunk;
        unitPositions[entityIndexInQuery] = localToWorld.Position;
        unitIds[entityIndexInQuery] = entity.TeamIndex;

    }
}