using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;


public class TargetingEnemyUnitAuthoring : UnityEngine.MonoBehaviour
{
    public class TargetingEnemyUnitBaker : Baker<TargetingEnemyUnitAuthoring>
    {
        public override void Bake(TargetingEnemyUnitAuthoring authoring)
        {
           
        }
    }
}


public struct TargetingEnemyUnitComponentData : IComponentData
{
    public Entity targetingEntity;
}


public readonly partial struct TargetingEnemyUnitAspect : IAspect
{
    readonly TransformAspect transform;
    public readonly Entity self;
    readonly RefRO<TeamUnitComponentData> teamUnit;
    readonly RefRW<TargetingEnemyUnitComponentData> targeting;

    public float3 WorldPosition => transform.WorldPosition;
    public int TeamIndex => teamUnit.ValueRO.TeamIndex;

    public Entity Targeting
    {
        set => targeting.ValueRW.targetingEntity = value;
    }
}


[BurstCompile]
public partial struct TargetingEnemyUnitSystem : ISystem
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
        //unitQuery.setfil
        var unitCount = unitQuery.CalculateEntityCount();
        var world = state.WorldUnmanaged;
        var delta = SystemAPI.Time.DeltaTime;

        var copyUnitPositions = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
        var copyUnitIds = CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
        var copyUnitTeamIndices = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
        var unitChunkBaseEntityIndexArray = unitQuery.CalculateBaseEntityIndexArrayAsync(
            world.UpdateAllocator.ToAllocator, state.Dependency,
            out var unitChunkBaseIndexJobHandle);

        var copyTargetJob = new InitialTargetEnemyUnitJob
        {
            chunkBaseEntitiyIndices = unitChunkBaseEntityIndexArray,
            unitIds = copyUnitIds,
            unitPositions = copyUnitPositions,
            unitTeamIndices = copyUnitTeamIndices,
        };

        var copyTargetHandle = copyTargetJob.ScheduleParallel(unitQuery, unitChunkBaseIndexJobHandle);


        var job = new TargetingEnemyUnitJob
        {
            teamIndices = copyUnitTeamIndices,
            unitIds = copyUnitIds,
            unitPositions = copyUnitPositions,

        };

        var targethandle = job.ScheduleParallel(unitQuery, copyTargetHandle);
        state.Dependency = targethandle;
    }
}


[BurstCompile]
public partial struct TargetingEnemyUnitJob : IJobEntity
{
    [ReadOnly] public NativeArray<float3> unitPositions;
    [ReadOnly] public NativeArray<Entity> unitIds;
    [ReadOnly] public NativeArray<int> teamIndices;

    void Execute(ref TargetingEnemyUnitAspect value)
    {
        var target = Entity.Null;

        var maxSq = float.MaxValue;

        for (var i = 0; i < unitIds.Length; i++)
        {
            var entity = teamIndices[i];
            if (entity == value.TeamIndex)
            {
                continue;
            }
            var entityPos = unitPositions[i];

            var nextSq = math.distancesq(value.WorldPosition, entityPos);
            if (maxSq > nextSq)
            {
                maxSq = nextSq;
                target = unitIds[i];
            }
        }

        value.Targeting = target;
        //value.
        //var dir = math.normalize(targetPos - value.WorldPosition) * Delta;
        //value.WorldPosition += (dir);


    }
}


[BurstCompile]
public partial struct InitialTargetEnemyUnitJob : IJobEntity
{

    [ReadOnly] public NativeArray<int> chunkBaseEntitiyIndices;
    [NativeDisableParallelForRestriction] public NativeArray<float3> unitPositions;
    [NativeDisableParallelForRestriction] public NativeArray<Entity> unitIds;
    [NativeDisableParallelForRestriction] public NativeArray<int> unitTeamIndices;

    void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk,
        in TargetingEnemyUnitAspect value)
    {
        var entityIndexInQuery = chunkBaseEntitiyIndices[chunkIndexInQuery] + entityIndexInChunk;
        unitPositions[entityIndexInQuery] = value.WorldPosition;
        unitIds[entityIndexInQuery] = value.self;
        unitTeamIndices[entityIndexInQuery] = value.TeamIndex;

    }
}
