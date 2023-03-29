using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct MovementSystem : ISystem
{
    private NativeArray<Vector3> clickedPosition;

    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");
        clickedPosition = new NativeArray<Vector3>(1, Allocator.Persistent);

        state.RequireForUpdate<MovementComponentData>();
        InputSystem.ClickAction.performed += OnMouseClick;
    }

    public void OnDestroy(ref SystemState state)
    {
        Debug.Log("MovementSystem OnDestroy!");
        InputSystem.ClickAction.performed -= OnMouseClick;

        clickedPosition.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        var job = new MovementJob
        {
            deltaTime = delta,
            destPoint = clickedPosition[0],
            epsilon = 0.5f
        };
        job.Schedule();
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        Debug.Log("MovementSystem OnMouseClick");

        if (Raycaster.ShootRay())
        {
            clickedPosition[0] = Raycaster.Hit.point;

            Debug.Log("MovementSystem OnRayHit: " + clickedPosition[0]);
        }

    }
}

[BurstCompile]
public partial struct MovementJob : IJobEntity
{
    [ReadOnly] public float3 destPoint;
    [ReadOnly] public float deltaTime;

    [ReadOnly] public float epsilon;

    void Execute(ref MovementAspect aspect, in MovementComponentData component)
    {
        if (math.distancesq(aspect.LocalPosition, destPoint) > epsilon)
        {
            aspect.MoveToPointOnlyXZ(destPoint, deltaTime);
        }
    }
}