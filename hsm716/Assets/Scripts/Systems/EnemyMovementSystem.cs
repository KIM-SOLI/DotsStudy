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
using UnityEngine;
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
        enemyQuery = SystemAPI.QueryBuilder().WithAll<Soldier, EnemyTag>().Build();
        state.RequireForUpdate(enemyQuery);
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

        new EnemyMoveJob
        {
            dt = dt,
            ecb = ecb
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct EnemyMoveJob : IJobEntity
    {
        public float dt;
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        private void Execute(SoldierAspect soldier)
        {
            soldier.EnemyMove(dt);
        }
    }
}