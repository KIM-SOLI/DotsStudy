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
    static Vector3 clickedPosition;

    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");
        InputSystem.ClickAction.performed += OnMouseClick;

        state.RequireForUpdate<MovementComponentData>();
    }

    public void OnDestroy(ref SystemState state)
    {
        InputSystem.ClickAction.performed -= OnMouseClick;
        Debug.Log("MovementSystem OnDestroy!");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        var job = new MovementJob
        {
            deltaTime = delta,
            destPoint = clickedPosition,
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
    //[ReadOnly] public Vector3 destPoint;
    [ReadOnly] public float3 destPoint;
    [ReadOnly] public float deltaTime;

    void Execute(ref MovementAspect aspect)
    {
        float3 dir = destPoint - aspect.WorldPosition;

        aspect.WorldPosition += dir * deltaTime;
    }
}