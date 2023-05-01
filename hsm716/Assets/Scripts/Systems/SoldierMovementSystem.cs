using System;
using System.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.

[BurstCompile]
partial struct SoliderMovementSystem : ISystem
{
    EntityQuery soldierQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var mySoldierQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
               .WithAll<MySoldierTag>()
               .WithAll<Soldier>()
               .WithAll<MoveToTarget>();
               //.WithAll<AttackToTarget>()
               //.WithAll<LifeStateTag>();
        soldierQuery = state.GetEntityQuery(mySoldierQueryBuilder);
        state.RequireForUpdate(soldierQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new SoldierMoveJob
        {
            localTransformHandles = state.GetComponentTypeHandle<LocalTransform>(true),
            m_Soldier = state.GetComponentTypeHandle<Soldier>(true),
            targetComponents = state.GetComponentTypeHandle<MoveToTarget>(true),
            lifeStateComponent = state.GetComponentTypeHandle<LifeStateTag>(true),
            attackTargetComponent = state.GetComponentTypeHandle<AttackToTarget>(true),
            enemyTagComponent = state.GetComponentTypeHandle<MySoldierTag>(true),
            entityHandle = state.GetEntityTypeHandle(),
            writter = ecb,
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        state.Dependency = job.ScheduleParallel(soldierQuery, state.Dependency);

        //var tankTransform = SystemAPI.GetComponent<LocalToWorld>();
    }

    [BurstCompile]
    public partial struct SoldierMoveJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformHandles;
        [ReadOnly] public ComponentTypeHandle<Soldier> m_Soldier;
        [ReadOnly] public ComponentTypeHandle<MoveToTarget> targetComponents;
        [ReadOnly] public ComponentTypeHandle<AttackToTarget> attackTargetComponent;
        [ReadOnly] public ComponentTypeHandle<LifeStateTag> lifeStateComponent;
        [ReadOnly] public ComponentTypeHandle<MySoldierTag> enemyTagComponent;
        [ReadOnly] public float deltaTime;

        public EntityTypeHandle entityHandle;

        public EntityCommandBuffer.ParallelWriter writter;


        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(entityHandle);
            var loalTransforms = chunk.GetNativeArray(ref localTransformHandles);
            NativeArray<MoveToTarget> targetComponentList = chunk.GetNativeArray(ref targetComponents);
            
            var enumerator = new ChunkEntityEnumerator(
                useEnabledMask,
                chunkEnabledMask,
                chunk.Count);

            while(enumerator.NextEntityIndex(out var index))
            {
                var entity = entities[index];
                var loalTransform = loalTransforms[index];
                var target = targetComponentList[index];
           

                // Notice that this is a lambda being passed as parameter to ForEach.
                float3 targetPosition = target.targetPosition;
                var localPosition = loalTransform.Position;
                float3 moveDirection = math.normalize(targetPosition - localPosition);

                if (Vector3.Distance(targetPosition, localPosition) > 1f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(moveDirection, math.up());
                    
                    loalTransform.Position += moveDirection * deltaTime * target.moveSpeed;
                    loalTransform.Rotation = lookRotation;
                    writter.SetComponent(unfilteredChunkIndex,entity, loalTransform);
                    
                }
                //loalTransform.ro LookAt(targetPosition);
            }

           


        }
    }
}