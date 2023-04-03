using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample1
{


    public class RangedWeponMoveSystemAuthoring : IGetBakedSystem
    {
        public RangedWeponMoveSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(RangedWeponMoveSystem);
        }
    }

    [DisableAutoCreation]
    [BurstCompile]
    public partial struct RangedWeponMoveSystem : ISystem
    {
        EntityQuery unitQuery;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RangedWeaponComponentData>()
                .WithAll<MovableUnitComponentData>()
                .WithAll<EnemyTargetComponentData>();

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
            var targetPositions = state.GetComponentLookup<LocalToWorld>(false);

            var job = new RangedWeaponSetMovePositionJob
            {
                targetPositions = targetPositions,

            };
            var setpositionHandle = job.ScheduleParallel(state.Dependency);


            var moveJob = new RangedWeaponMoveToTargetJob
            {

            };

            var moveHandle = moveJob.ScheduleParallel(setpositionHandle);

            state.Dependency = moveHandle;
        }
    }



    [BurstCompile]
    public partial struct RangedWeaponSetMovePositionJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> targetPositions;

        [BurstCompile]
        void Execute(ref RangedWeaponUnitAspect value)
        {
            if (targetPositions.HasComponent(value.targetEntity))
            {
                var pos = targetPositions.GetRefRO(value.targetEntity);
                var targetPos = pos.IsValid ? pos.ValueRO.Position : value.WorldPosition;

                value.targetPosition = targetPos;
            }
        }
    }


    [BurstCompile]
    public partial struct RangedWeaponMoveToTargetJob : IJobEntity
    {
        [BurstCompile]
        void Execute(ref RangedWeaponUnitAspect value)
        {
            var targetPos = value.targetPosition;

            var distanceSq = math.distancesq(value.WorldPosition, targetPos);
            var rangeSq = math.pow(value.Range, 2);
            if (rangeSq < distanceSq)
            {
                value.WorldPosition += math.normalize(targetPos - value.WorldPosition) * value.Speed;
            }
        }
    }
}
