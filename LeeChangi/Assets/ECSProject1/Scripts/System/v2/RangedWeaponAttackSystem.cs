using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample1
{


    public class RangedWeaponAttackSystemAuthoring : IGetBakedSystem
    {
        public RangedWeaponAttackSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(RangedWeaponAttackSystem);
        }
    }


    [UpdateAfter(typeof(RangedWeponMoveSystem))]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct RangedWeaponAttackSystem : ISystem
    {
        ComponentLookup<EnemyTargetComponentData> lookupTargetEnemy;
        ComponentLookup<BodyStat> lookupBodies;

        EntityQuery unitQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RangedWeaponComponentData>()
                .WithAll<EnemyTargetComponentData>()
                .WithAll<LockOnTargetComponentData>();


            unitQuery = state.GetEntityQuery(unitQueryBuilder);


            lookupTargetEnemy = state.GetComponentLookup<EnemyTargetComponentData>(true);
            lookupBodies = state.GetComponentLookup<BodyStat>();

            state.RequireForUpdate(unitQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            lookupBodies.Update(ref state);
            lookupTargetEnemy.Update(ref state);

            var unitIds = unitQuery.ToEntityArray(Allocator.TempJob);
            var targetbodies = new NativeArray<BodyStat>(unitIds.Length, Allocator.TempJob);

            var deltaTime = SystemAPI.Time.DeltaTime;
            var job = new CollectLockOnTargetJob
            {
                entities = unitIds,
                lookupBodies = lookupBodies,
                lookupEnemyTarget = lookupTargetEnemy,
                targetBodies = targetbodies
            };
            job.Schedule(unitQuery);

            //unitIds.Dispose();
            targetbodies.Dispose();
        }
    }



    public readonly partial struct RangedWeaponAttackAspect : IAspect
    {
        readonly RefRW<LocalTransform> Transform;
        readonly RefRO<RangedWeaponComponentData> rangedWeapon;
        readonly RefRW<EnemyTargetComponentData> target;

        public Entity targetEntity => target.ValueRO.target;
        public float Range => rangedWeapon.ValueRO.SafeDistance;
        public float3 targetPosition
        {
            get => target.ValueRO.targetPosition;
        }

        public float3 WorldPosition
        {
            get => Transform.ValueRO.Position;
        }
    }


    [BurstCompile]
    public partial struct CollectLockOnTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<BodyStat> lookupBodies;
        [ReadOnly] public ComponentLookup<EnemyTargetComponentData> lookupEnemyTarget;
        [ReadOnly] public NativeArray<Entity> entities;

        [NativeDisableParallelForRestriction] public NativeArray<BodyStat> targetBodies;

        [BurstCompile]
        public void Execute(in RangedWeaponAttackAspect value)
        {
            if (value.targetEntity != Entity.Null)
            {

                if (lookupBodies.HasComponent(value.targetEntity))
                {
                    var copyComp = lookupBodies[value.targetEntity];
                    copyComp.HP -= 1;
                    lookupBodies[value.targetEntity] = copyComp;
                }
            }

            //for (var i = 0; i < entities.Length; i++)
            //{
            //    var entity = entities[i];
            //    var target = lookupEnemyTarget[entity].target;
            //    targetBodies[i] = lookupBodies[target];
            //}
        }
    }


    //[BurstCompile]
    //public partial struct RangedWeaponTargetAttackJob : IJobEntity
    //{
    //    [ReadOnly] public NativeArray<BodyStat> targetBodies;


    //    [BurstCompile]
    //    void Execute(ref RangedWeaponAttackAspect value)
    //    {

    //        if (targetBodies.HasComponent(value.targetEntity))
    //        {
    //            var body = targetBodies.GetRefRO(value.targetEntity);
    //            body.ValueRO.HP
    //            var targetPos = pos.IsValid ? pos.ValueRO.Position : value.WorldPosition;

    //            value.targetPosition = targetPos;
    //        }
    //    }
    //}

}