using System;
using System.Net;
using Unity.Burst;
using Unity.Collections;
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

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.

[BurstCompile]
partial struct SoliderAttackSystem : ISystem
{
    EntityQuery soldierQuery;
    EntityQuery enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        soldierQuery = state.GetEntityQuery(ComponentType.ReadOnly<MySoldierTag>());
        enemyQuery = state.GetEntityQuery(ComponentType.ReadOnly<EnemyTag>());
        state.RequireForUpdate(soldierQuery);
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

        float3 targetPosition = new float3(123456789, 123456789, 123456789);
        var entityArray = enemyQuery.ToEntityArray(Allocator.TempJob);
        var entityManager = state.World.EntityManager;

        float minDist = 100000000f;
        foreach (var entity in entityArray)
        {
            var position = entityManager.GetComponentData<LocalTransform>(entity).Position;
            float dist = math.distancesq(targetPosition, position);
            if(dist < minDist)
            {
                minDist = dist;
                targetPosition = position;
            }
        }
        entityArray.Dispose();
        new SoldierAttackJob
        {
            dt = dt,
            ecb = ecb,
            targetPosition = targetPosition
        }.ScheduleParallel();

        //var tankTransform = SystemAPI.GetComponent<LocalToWorld>();
    }

    [BurstCompile]
    public partial struct SoldierAttackJob : IJobEntity
    {
        public float dt;
        public EntityCommandBuffer.ParallelWriter ecb;
        public float3 targetPosition;
        [BurstCompile]
        private void Execute(SoldierAspect soldier)
        {
          

            if (soldier.IsInTargetRange(targetPosition,5))
            {
                Debug.Log("AAA");
            }
        }
    }
}