using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.EventSystems;

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


    public readonly partial struct RangedWeaponUnitAspect : IAspect
    {
        readonly RefRW<LocalTransform> Transform;
        readonly RefRO<RangedWeaponComponentData> rangedWeapon;
        readonly RefRO<MovableUnitComponentData> move;
        readonly RefRW<EnemyTargetComponentData> target;

        public readonly Entity Self;


        public float Range => rangedWeapon.ValueRO.SafeDistance;
        public float LockOnRange => rangedWeapon.ValueRO.LockOnRange;
        public float LockOffRange => rangedWeapon.ValueRO.LockOffRange;
        public float Speed => move.ValueRO.moveSpeed;
        public Entity targetEntity => target.ValueRO.target;

        public float3 targetPosition
        {
            get => target.ValueRO.targetPosition;
            set => target.ValueRW.targetPosition = value;
        }



        public float3 WorldPosition
        {
            get => Transform.ValueRO.Position;
            set => Transform.ValueRW.Position = value;
        }
    }



    [DisableAutoCreation]
    [BurstCompile]
    public partial struct RangedWeponMoveSystem : ISystem
    {
        EntityQuery unitQuery;
        ComponentLookup<LocalToWorld> targetPositions;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RangedWeaponComponentData>()
                .WithAll<EnemyTargetComponentData>();

            unitQuery = state.GetEntityQuery(unitQueryBuilder);
            state.RequireForUpdate(unitQuery);
            targetPositions = state.GetComponentLookup<LocalToWorld>(false);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            targetPositions.Update(ref state);

            var job = new RangedWeaponSetMovePositionJob
            {
                targetPositions = targetPositions,

            };
            var setpositionHandle = job.Schedule(state.Dependency);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);


            var moveJob = new RangedWeaponMoveToTargetJob
            {
                DeltaTime = deltaTime,
                ecb = ecb,
            };

            var moveHandle = moveJob.Schedule(setpositionHandle);

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
        [ReadOnly] public float DeltaTime;
        public EntityCommandBuffer ecb;

        [BurstCompile]
        void Execute(ref RangedWeaponUnitAspect value)
        {
            var targetPos = value.targetPosition;

            var distanceSq = math.distancesq(value.WorldPosition, targetPos);
            var rangeSq = math.pow(value.Range, 2);
            if (rangeSq < distanceSq)
            {
                if (rangeSq < math.pow(value.LockOnRange, 2))
                {
                    ecb.SetComponentEnabled(value.Self, ComponentType.ReadOnly<LockOnTargetComponentData>(), true);
                }
                else if (rangeSq > math.pow(value.LockOffRange, 2))
                {
                    ecb.SetComponentEnabled(value.Self, ComponentType.ReadOnly<LockOnTargetComponentData>(), false);
                }

                value.WorldPosition += (math.normalize(targetPos - value.WorldPosition) * value.Speed * DeltaTime);
            }
        }
    }
}
