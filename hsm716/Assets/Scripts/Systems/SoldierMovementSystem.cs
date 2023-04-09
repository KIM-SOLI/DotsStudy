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
partial struct SoliderMovementSystem : ISystem
{
    EntityQuery soldierQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        soldierQuery = state.GetEntityQuery(ComponentType.ReadOnly<MySoldierTag>());
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

        new SoldierMoveJob
        {
            dt = dt,
            ecb = ecb
        }.ScheduleParallel();

        //var tankTransform = SystemAPI.GetComponent<LocalToWorld>();
    }

    [BurstCompile]
    public partial struct SoldierMoveJob : IJobEntity
    {
        public float dt;
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        private void Execute(SoldierAspect soldier)
        {
            soldier.Move(dt);
        }
    }
}