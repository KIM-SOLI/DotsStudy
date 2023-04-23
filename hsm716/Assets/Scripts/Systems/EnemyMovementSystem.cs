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
using static SoliderMovementSystem;
using static UnityEngine.GraphicsBuffer;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.

[BurstCompile]
partial struct EnemyMovementSystem : ISystem
{
    EntityQuery enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        using var enemyQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
       .WithAll<EnemyTag>()
       .WithAll<Soldier>()
       .WithAll<MoveToTarget>();
        //.WithAll<AttackToTarget>()
        //.WithAll<LifeStateTag>();
        enemyQuery = state.GetEntityQuery(enemyQueryBuilder);
        state.RequireForUpdate(enemyQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new EnemyMoveJob
        {
            localTransformHandles = state.GetComponentTypeHandle<LocalTransform>(true),
            m_Soldier = state.GetComponentTypeHandle<Soldier>(true),
            targetComponents = state.GetComponentTypeHandle<MoveToTarget>(true),
            lifeStateComponent = state.GetComponentTypeHandle<LifeStateTag>(true),
            enemyTagComponent = state.GetComponentTypeHandle<EnemyTag>(true),
            entityHandle = state.GetEntityTypeHandle(),
            writter = ecb,
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        state.Dependency = job.ScheduleParallel(enemyQuery, state.Dependency);
    }

    [BurstCompile]
    public partial struct EnemyMoveJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformHandles;
        [ReadOnly] public ComponentTypeHandle<Soldier> m_Soldier;
        [ReadOnly] public ComponentTypeHandle<MoveToTarget> targetComponents;
        [ReadOnly] public ComponentTypeHandle<LifeStateTag> lifeStateComponent;
        [ReadOnly] public ComponentTypeHandle<EnemyTag> enemyTagComponent;
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

            while (enumerator.NextEntityIndex(out var index))
            {
                var entity = entities[index];
                var loalTransform = loalTransforms[index];
                var target = targetComponentList[index];

                // Notice that this is a lambda being passed as parameter to ForEach.
                float3 targetPosition = target.targetPosition;
                var localPosition = loalTransform.Position;
                float3 moveDirection = targetPosition;

                Quaternion lookRotation = Quaternion.LookRotation(targetPosition, math.up());

                loalTransform.Position += targetPosition * deltaTime * target.moveSpeed;
                //loalTransform.Position = new float3(localPosition.x, 0.01f, localPosition.z);
                loalTransform.Rotation = lookRotation;
                writter.SetComponent(unfilteredChunkIndex, entity, loalTransform);
                //loalTransform.ro LookAt(targetPosition);
            }

        }
    }
}