using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Sample1;

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
    readonly RefRO<LocalToWorld> transform;
    public readonly Entity self;
    readonly RefRO<TeamUnitComponentData> teamUnit;

    public float3 WorldPosition => transform.ValueRO.Position;
    public int TeamIndex => teamUnit.ValueRO.TeamIndex;

    
}

//[DisableAutoCreation]
//[BurstCompile]
//public partial struct TargetingEnemyUnitSystem : ISystem
//{
//    EntityQuery unitQuery;
//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        //state.Enabled = false;
//        using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
//            .WithAll<TeamUnitComponentData>()
//            .WithNone<EnemyTargetComponentData>()
//            .WithAllRW<LocalToWorld>();
//        unitQuery = state.GetEntityQuery(unitQueryBuilder);
//        state.RequireForUpdate(unitQuery);
//    }

//    [BurstCompile]
//    public void OnDestroy(ref SystemState state)
//    {
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        //unitQuery.setfil
//        var unitCount = unitQuery.CalculateEntityCount();
//        var world = state.WorldUnmanaged;
//        var delta = SystemAPI.Time.DeltaTime;

//        var copyUnitPositions = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
//        var copyUnitIds = CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
//        var copyUnitTeamIndices = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(unitCount, ref world.UpdateAllocator);
//        var unitChunkBaseEntityIndexArray = unitQuery.CalculateBaseEntityIndexArrayAsync(
//            world.UpdateAllocator.ToAllocator, state.Dependency,
//            out var unitChunkBaseIndexJobHandle);

//        var copyTargetJob = new InitialTargetEnemyUnitJob
//        {
//            chunkBaseEntitiyIndices = unitChunkBaseEntityIndexArray,
//            unitIds = copyUnitIds,
//            unitPositions = copyUnitPositions,
//            unitTeamIndices = copyUnitTeamIndices,
//        };

//        var copyTargetHandle = copyTargetJob.ScheduleParallel(unitQuery, unitChunkBaseIndexJobHandle);
//        //var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
//        //var ecb = ecbSingleton.CreateCommandBuffer(world);

//        //var job = new TargetingEnemyUnitJob
//        //{
//        //    teamIndices = copyUnitTeamIndices,
//        //    unitIds = copyUnitIds,
//        //    unitPositions = copyUnitPositions,
//        //    ecb = ecb,
//        //};
        
//        //var targethandle = job.ScheduleParallel(unitQuery, copyTargetHandle);
//        state.Dependency = copyTargetHandle;
//    }
//}
