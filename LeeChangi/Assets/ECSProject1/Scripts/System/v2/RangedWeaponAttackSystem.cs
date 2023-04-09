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
            state.RequireForUpdate(unitQuery);

            lookupTargetEnemy = state.GetComponentLookup<EnemyTargetComponentData>(true);
            lookupBodies = state.GetComponentLookup<BodyStat>();
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
                loockupBodies = lookupBodies,
                lookupEnemyTarget = lookupTargetEnemy,
                targetBodies = targetbodies
            };
            var handle = job.Schedule(unitIds.Length,4);
            
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
    public partial struct CollectLockOnTargetJob : IJobParallelFor
    {
        [ReadOnly] public ComponentLookup<BodyStat> loockupBodies;
        [ReadOnly] public ComponentLookup<EnemyTargetComponentData> lookupEnemyTarget;
        [ReadOnly] public NativeArray<Entity> entities;

        [NativeDisableParallelForRestriction] public NativeArray<BodyStat> targetBodies;

        [BurstCompile]
        public void Execute(int index)
        {
            var entity = entities[index];
            var target = lookupEnemyTarget[entity].target;
            targetBodies[index] = loockupBodies[target];
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