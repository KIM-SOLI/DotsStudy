using System;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


[BurstCompile]
partial struct SoliderAttackSystem : ISystem
{
    EntityQuery soldierQuery;
    EntityQuery enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var mySoldierQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
       .WithAll<MySoldierTag>()
       .WithAll<Soldier>()
       .WithAll<MoveToTarget>();
        
        soldierQuery = state.GetEntityQuery(mySoldierQueryBuilder);
        state.RequireForUpdate(soldierQuery);


        using var enemyQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
       .WithAll<EnemyTag>()
       .WithAll<Soldier>()
       .WithAll<MoveToTarget>();
       
        enemyQuery = state.GetEntityQuery(enemyQueryBuilder);
        state.RequireForUpdate(enemyQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        NativeArray<Entity> enemyList = soldierQuery.ToEntityArray(Allocator.Temp);
        var job = new SoldierAttackJob
        {
            localTransformHandles = state.GetComponentTypeHandle<LocalTransform>(true),
            m_Soldier = state.GetComponentTypeHandle<Soldier>(true),
            targetComponents = state.GetComponentTypeHandle<MoveToTarget>(true),
            lifeStateComponent = state.GetComponentTypeHandle<LifeStateTag>(true),
            attackTargetComponents = state.GetComponentTypeHandle<AttackToTarget>(true),
            mySoldierTagComponent = state.GetComponentTypeHandle<MySoldierTag>(true),
            entityHandle = state.GetEntityTypeHandle(),

            deltaTime = dt,
            writter = ecb
        };
        state.Dependency = job.ScheduleParallel(soldierQuery, state.Dependency);
    }

    [BurstCompile]
    public partial struct SoldierAttackJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformHandles;
        [ReadOnly] public ComponentTypeHandle<Soldier> m_Soldier;
        [ReadOnly] public ComponentTypeHandle<MoveToTarget> targetComponents;
        [ReadOnly] public ComponentTypeHandle<AttackToTarget> attackTargetComponents;
        [ReadOnly] public ComponentTypeHandle<LifeStateTag> lifeStateComponent;
        [ReadOnly] public ComponentTypeHandle<MySoldierTag> mySoldierTagComponent;
        [ReadOnly] public float deltaTime;

        public EntityTypeHandle entityHandle;
        public EntityCommandBuffer.ParallelWriter writter;


        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
            var loalTransforms = chunk.GetNativeArray(ref localTransformHandles);
            NativeArray<MoveToTarget> targetComponentList = chunk.GetNativeArray(ref targetComponents);
            NativeArray<AttackToTarget> attackToTargetComponentList = chunk.GetNativeArray(ref attackTargetComponents);
            NativeArray<LifeStateTag> lifeStateTags = chunk.GetNativeArray(ref lifeStateComponent);
            
            var enumerator = new ChunkEntityEnumerator(
                useEnabledMask,
                chunkEnabledMask,
                chunk.Count);


            while (enumerator.NextEntityIndex(out var index))
            {
                var entity = entities[index];
                var loalTransform = loalTransforms[index];
                var target = attackToTargetComponentList[index];
                var life = lifeStateTags[index];

                float3 targetPosition = target.targetPosition;
                var localPosition = loalTransform.Position;
                localPosition.y = 0;
                if (math.distancesq(localPosition, targetPosition) < 2f * loalTransform.Scale)
                {
                    if (loalTransform.Scale >= target.targetScale)
                    {
                        life.level += 1;
                        loalTransform.Scale += 1;
                        writter.SetComponent(unfilteredChunkIndex, entity, loalTransform);
                        writter.SetComponent(unfilteredChunkIndex, entity, life);
                        writter.DestroyEntity(unfilteredChunkIndex, target.targetEntity);
                    }
                    else
                    {
                        writter.DestroyEntity(unfilteredChunkIndex, entity);
                    }
                }

            }
        }
    }
}