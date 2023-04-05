using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample1
{


	public class FindTargetSystemSystemAuthoring : IGetBakedSystem
	{
		public FindTargetSystemSystemAuthoring(){}
		public Type GetSystemType()
		{
			return typeof(FindTargetSystemSystem);
		}
	}

    [UpdateAfter(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(UnitSpawnSystem))]
	[DisableAutoCreation]
	[BurstCompile]
	public partial struct FindTargetSystemSystem : ISystem
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

            //ComponentLookup<>
            var unitCount = unitQuery.CalculateEntityCount();
            var world = state.WorldUnmanaged;

                    
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

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(world);
            var job = new TargetingEnemyUnitJob
            {
                teamIndices = copyUnitTeamIndices,
                unitIds = copyUnitIds,
                unitPositions = copyUnitPositions,
                ecb = ecb,
            };

            var targethandle = job.Schedule(copyTargetHandle);
            state.Dependency = targethandle;
        }
	}


    [BurstCompile]
    public partial struct TargetingEnemyUnitJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> unitPositions;
        [ReadOnly] public NativeArray<Entity> unitIds;
        [ReadOnly] public NativeArray<int> teamIndices;
        public EntityCommandBuffer ecb;

        [BurstCompile]
        void Execute(in TeamUnitAspect value)
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


            if (target != Entity.Null)
            {
                ecb.SetComponent(value.Self, new EnemyTargetComponentData
                {
                    target = target,
                });
                ecb.SetComponentEnabled(value.Self, typeof(EnemyTargetComponentData), true);

            }
            else
            {
                ecb.SetComponentEnabled(value.Self, typeof(EnemyTargetComponentData), false);
            }
        }
    }



    [BurstCompile]
    public partial struct InitialTargetEnemyUnitJob : IJobEntity
    {

        [ReadOnly] public NativeArray<int> chunkBaseEntitiyIndices;
        [NativeDisableParallelForRestriction] public NativeArray<float3> unitPositions;
        [NativeDisableParallelForRestriction] public NativeArray<Entity> unitIds;
        [NativeDisableParallelForRestriction] public NativeArray<int> unitTeamIndices;

        [BurstCompile]
        void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk,
            in TeamUnitAspect value)
        {
            var entityIndexInQuery = chunkBaseEntitiyIndices[chunkIndexInQuery] + entityIndexInChunk;
            unitPositions[entityIndexInQuery] = value.WorldPosition;
            unitIds[entityIndexInQuery] = value.Self;
            unitTeamIndices[entityIndexInQuery] = value.TeamIndex;

        }
    }



}

