using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements.Experimental;

namespace Sample1
{


    public class FindEnemySystemAuthoring : IGetBakedSystem
    {
        public FindEnemySystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(FindEnemySystem);
        }
    }

    [DisableAutoCreation]
    [BurstCompile]
    public partial struct FindEnemySystem : ISystem
    {
        EntityQuery unitQuery;
        EntityQuery totalUnitQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var targetterQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TeamUnitComponentData>()
                .WithNone<EnemyTargetComponentData>();
            using var totalQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TeamUnitComponentData>();

            unitQuery = state.GetEntityQuery(targetterQueryBuilder);
            totalUnitQuery = state.GetEntityQuery(totalQueryBuilder);
            state.RequireForUpdate(unitQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton =
            SystemAPI.GetSingleton<
                BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var totalEntity = totalUnitQuery.ToEntityArray(Allocator.TempJob);


            var job = new FindEnemyJob
            {
                entityTeams = state.GetComponentTypeHandle<TeamUnitComponentData>(true),
                entityPositions = state.GetComponentTypeHandle<LocalToWorld>(true),

                targetPositions = state.GetComponentLookup<LocalToWorld>(true),
                targetTeams = state.GetComponentLookup<TeamUnitComponentData>(true),

                totalEntities = totalEntity,

                entityHandle = state.GetEntityTypeHandle(),
                writer = ecb.AsParallelWriter(),
            };
            state.Dependency = job.ScheduleParallel(unitQuery, state.Dependency);
        }
    }



    [BurstCompile]
    public partial struct FindEnemyJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<TeamUnitComponentData> entityTeams;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> entityPositions;

        [ReadOnly] public ComponentLookup<TeamUnitComponentData> targetTeams;
        [ReadOnly] public ComponentLookup<LocalToWorld> targetPositions;
        [ReadOnly] public NativeArray<Entity> totalEntities;

        public EntityTypeHandle entityHandle;

        public EntityCommandBuffer.ParallelWriter writer;


        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities =
                   chunk.GetNativeArray(entityHandle);
            NativeArray<LocalToWorld> localToWorlds =
                chunk.GetNativeArray(ref entityPositions);
            NativeArray<TeamUnitComponentData> teamComponents =
                chunk.GetNativeArray(ref entityTeams);

            var enumerator = new ChunkEntityEnumerator(
                useEnabledMask,
                chunkEnabledMask,
                chunk.Count);

            var totalLength = totalEntities.Length;
            while (enumerator.NextEntityIndex(out var index))
            {
                var entity = entities[index];
                var localtoWorld = localToWorlds[index];
                var team = teamComponents[index];

                var compDistance = float.MaxValue;
                Entity minEntity = Entity.Null;
                float3 minEntityPos = float3.zero;
                for (var i = 0; i < totalLength; i++)
                {
                    var targetEntity = totalEntities[i];
                    if (!targetTeams.HasComponent(targetEntity))
                    { continue; }

                    if (!targetPositions.HasComponent(targetEntity))
                    { continue; }

                    var targetTeam = targetTeams[targetEntity];
                    if (team.TeamIndex != targetTeam.TeamIndex)
                    {
                        var targetPosition = targetPositions[targetEntity];
                        var dist = math.distancesq(localtoWorld.Position, targetPosition.Position);
                        if (compDistance > dist)
                        {
                            minEntity = targetEntity;
                            minEntityPos = targetPosition.Position;
                        }
                    }
                }

                if (minEntity != Entity.Null)
                {
                    writer.SetComponentEnabled<EnemyTargetComponentData>(unfilteredChunkIndex, entity, true);
                    writer.SetComponent(unfilteredChunkIndex, entity, new EnemyTargetComponentData
                    {
                        target = minEntity,
                        targetPosition = minEntityPos,
                    });
                }



            }
            //NativeArray<EnemyTargetComponentData> targetList =
            //	chunk.GetNativeArray(ref enemyTargets);
        }
    }

}