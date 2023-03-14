using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
partial struct MovementSystem : ISystem
{
    EntityQuery entityQuery;
    Vector3 clickedPosition;

    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");
        InputSystem.ClickAction.performed += OnMouseClick;

        state.RequireForUpdate<MovementComponentData>();

        using var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp).
            WithAll<MovementComponentData>().
            WithAllRW<LocalToWorld>();

        entityQuery = state.GetEntityQuery(entityQueryBuilder);
        state.RequireForUpdate(entityQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        Debug.Log("MovementSystem OnDestroy!");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        var job = new MovementJob
        {
            deltaTime = delta,
            destPoint = clickedPosition
        };

        job.Schedule();
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        Debug.Log("MovementSystem OnMouseClick");

        if (Raycaster.ShootRay())
        {
            //var rayInfo = Raycaster.LastRayInfo;
            clickedPosition = Raycaster.Hit.point;
            Debug.Log("MovementSystem OnRayHit: " + clickedPosition);
        }

    }
}

[BurstCompile]
public partial struct MovementJob : IJobEntity
{
    [ReadOnly] public float3 destPoint;
    [ReadOnly] public float deltaTime;

    [BurstCompile]
    void Execute(ref LocalTransform localTransform, in MovementComponentData value)
    {
        float3 dir = localTransform.Position - destPoint;
        localTransform.Position.xz += dir.xz * deltaTime;
    }
}