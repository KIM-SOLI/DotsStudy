using System;
using System.Net;
using TMPro;
using Unity.Burst;
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

        new SoldierAttackJob
        {
            dt = dt,
            ecb = ecb,
        }.ScheduleParallel();

        //var tankTransform = SystemAPI.GetComponent<LocalToWorld>();
    }

    [BurstCompile]
    public partial struct SoldierAttackJob : IJobEntity
    {
        public float dt;
        public EntityCommandBuffer.ParallelWriter ecb;

        // 추가: soldier 엔티티
        public Entity soldierEntity;

        [BurstCompile]
        public void Execute(ref SoldierAspect soldier)
        {
            // 가장 가까운 적이 사정거리 안에 있다면 공격
            if (soldier.IsInTargetRange(2))
            {
                Debug.Log("Soldier is attacking closest enemy!");
                soldier.KillEnemy(soldier.GetAttackTargetEntity());
            }
        }
    }
}