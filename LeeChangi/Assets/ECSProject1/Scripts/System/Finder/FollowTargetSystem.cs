using System;
using System.Numerics;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample1
{


    public class FollowTargetSystemAuthoring : IGetBakedSystem
    {
        public FollowTargetSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(FollowTargetSystem);
        }
    }

    [DisableAutoCreation]
    [BurstCompile]
    public partial struct FollowTargetSystem : ISystem
    {
        EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
              .WithAll<LocalToWorld>()
              .WithAll<EnemyTargetComponentData>()
              .WithAll<RangedWeaponComponentData>()
              .WithAll<MovableUnitComponentData>();
            
            entityQuery = state.GetEntityQuery(entityQueryBuilder);


            state.RequireForUpdate(entityQuery);
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


            var job = new FollowTargetJob
            {
                entityEnemyTarget = state.GetComponentTypeHandle<EnemyTargetComponentData>(true),
                entityHandle = state.GetEntityTypeHandle(),
                movableUnitHandles = state.GetComponentTypeHandle<MovableUnitComponentData>(true),
                rangedWeaponHandles = state.GetComponentTypeHandle<RangedWeaponComponentData>(true),
                targetPositions = state.GetComponentLookup<LocalTransform>(true),
                localTransformHandles = state.GetComponentTypeHandle<LocalTransform>(true),
                
                writer = ecb.AsParallelWriter(),
                deltaTime = SystemAPI.Time.DeltaTime,
            };
            state.Dependency = job.ScheduleParallel(entityQuery, state.Dependency);
        }
    }


    [BurstCompile]
    public partial struct FollowTargetJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<EnemyTargetComponentData> entityEnemyTarget;
        //[ReadOnly] public ComponentTypeHandle<LocalToWorld> entityPositions;
        [ReadOnly] public ComponentTypeHandle<RangedWeaponComponentData> rangedWeaponHandles;
        [ReadOnly] public ComponentTypeHandle<MovableUnitComponentData> movableUnitHandles;
        [ReadOnly] public ComponentLookup<LocalTransform> targetPositions;
        [ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformHandles;
        [ReadOnly] public float deltaTime;

        public EntityTypeHandle entityHandle;
        public EntityCommandBuffer.ParallelWriter writer;

        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities =
                   chunk.GetNativeArray(entityHandle);
            //var myPositions = chunk.GetNativeArray(ref entityPositions);
            var targets = chunk.GetNativeArray(ref entityEnemyTarget);
            var rangedWeapons = chunk.GetNativeArray(ref rangedWeaponHandles);
            var movableUnits = chunk.GetNativeArray(ref movableUnitHandles);
            var loalTransforms = chunk.GetNativeArray(ref localTransformHandles);

            var enumerator = new ChunkEntityEnumerator(
                useEnabledMask,
                chunkEnabledMask,
                chunk.Count);

            while (enumerator.NextEntityIndex(out var index))
            {
                var entity = entities[index];

                var target = targets[index].target;
                var ragneweapon = rangedWeapons[index];
                var movable = movableUnits[index];
                var local = loalTransforms[index];

                var targetPos = targetPositions[target];
                if (math.distancesq(targetPos.Position, local.Position) > ragneweapon.MaximumDistanceSq)
                {
                    var dir = math.normalize(targetPos.Position - local.Position) * movable.moveSpeed * deltaTime;
                    local.Position += dir;
                    writer.SetComponent(unfilteredChunkIndex, entity, local);
                }

            }
        }
    }
}