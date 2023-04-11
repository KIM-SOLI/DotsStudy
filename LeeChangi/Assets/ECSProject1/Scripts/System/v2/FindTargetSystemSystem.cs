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
        public FindTargetSystemSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(FindTargetSystemSystem);
        }
    }

    //[UpdateAfter(typeof(TransformSystemGroup))]
    //[UpdateAfter(typeof(UnitSpawnSystem))]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct FindTargetSystemSystem : ISystem
    {
        EntityQuery totalUnitQuery;
        EntityQuery targetterUnitQuery;
        ComponentLookup<TeamUnitComponentData> teamIndices;
        ComponentLookup<LocalTransform> unitPositions;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var totalUnitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
           .WithAll<TeamUnitComponentData>();
            using var targetterQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TeamUnitComponentData>()
                .WithNone<EnemyTargetComponentData>();

            totalUnitQuery = state.GetEntityQuery(totalUnitQueryBuilder);
            targetterUnitQuery = state.GetEntityQuery(targetterQueryBuilder);

            state.RequireForUpdate(targetterUnitQuery);

            teamIndices = state.GetComponentLookup<TeamUnitComponentData>();
            unitPositions = state.GetComponentLookup<LocalTransform>();

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            unitPositions.Update(ref state);
            teamIndices.Update(ref state);

            var totalUnitIds = totalUnitQuery.ToEntityArray(Allocator.TempJob);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var world = state.WorldUnmanaged;
            var ecb = ecbSingleton.CreateCommandBuffer(world);
            var job = new TargetingEnemyUnitJob
            {
                teamIndices = teamIndices,
                unitIds = totalUnitIds,
                unitPositions = unitPositions,
                ecb = ecb,
            };

            job.Schedule(targetterUnitQuery);

            //unitIds.Dispose();
        }
    }


    public readonly partial struct TeamUnitAspect : IAspect
    {
        readonly RefRW<TeamUnitComponentData> _unit;
        readonly RefRW<LocalTransform> Transform;

        //readonly RefRW<LocalTransform> localTransform;

        public readonly Entity Self;

        public int TeamIndex
        {
            get => _unit.ValueRO.TeamIndex;
        }

        public float3 WorldPosition
        {
            get => Transform.ValueRO.Position;
            set => Transform.ValueRW.Position = value;
        }
    }


    [BurstCompile]
    public partial struct TargetingEnemyUnitJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> unitPositions;
        [ReadOnly] public ComponentLookup<TeamUnitComponentData> teamIndices;
        [ReadOnly] public NativeArray<Entity> unitIds;
        public EntityCommandBuffer ecb;

        [BurstCompile]
        void Execute(in TeamUnitAspect value)
        {
            var target = Entity.Null;

            var maxSq = float.MaxValue;

            for (var i = 0; i < unitIds.Length; i++)
            {
                var entity = unitIds[i];

                if (!teamIndices.HasComponent(entity))
                {
                    continue;
                }

                var other = teamIndices[entity];
                if (other.TeamIndex == value.TeamIndex)
                {
                    continue;
                }

                if (!unitPositions.HasComponent(entity))
                {
                    continue;
                }

                var entityPos = unitPositions[entity];

                var nextSq = math.distancesq(value.WorldPosition, entityPos.Position);
                if (maxSq > nextSq)
                {
                    maxSq = nextSq;
                    target = entity;
                }
            }


            if (target != Entity.Null)
            {
                ecb.SetComponentEnabled(value.Self, ComponentType.ReadOnly<EnemyTargetComponentData>(), true);
                ecb.SetComponent(value.Self, new EnemyTargetComponentData
                {
                    target = target,
                });
            }
        }
    }



    //[BurstCompile]
    //public partial struct InitialTargetEnemyUnitJob : IJobEntity
    //{

    //    [ReadOnly] public NativeArray<int> chunkBaseEntitiyIndices;
    //    [NativeDisableParallelForRestriction] public NativeArray<float3> unitPositions;
    //    [NativeDisableParallelForRestriction] public NativeArray<Entity> unitIds;
    //    [NativeDisableParallelForRestriction] public NativeArray<int> unitTeamIndices;

    //    [BurstCompile]
    //    void Execute([ChunkIndexInQuery] int chunkIndexInQuery, [EntityIndexInChunk] int entityIndexInChunk,
    //        in TeamUnitAspect value)
    //    {
    //        var entityIndexInQuery = chunkBaseEntitiyIndices[chunkIndexInQuery] + entityIndexInChunk;
    //        unitPositions[entityIndexInQuery] = value.WorldPosition;
    //        unitIds[entityIndexInQuery] = value.Self;
    //        unitTeamIndices[entityIndexInQuery] = value.TeamIndex;

    //    }
    //}



}

