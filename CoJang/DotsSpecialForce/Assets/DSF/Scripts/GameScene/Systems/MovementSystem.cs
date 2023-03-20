using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
partial struct MovementSystem : ISystem
{
    Vector3 clickedPosition;
    private ComponentLookup<WorldTransform> m_WorldTransformLookup;

    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");
        InputSystem.ClickAction.performed += OnMouseClick;

        state.RequireForUpdate<MovementComponentData>();
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
        InputSystem.ClickAction.performed -= OnMouseClick;
        Debug.Log("MovementSystem OnDestroy!");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookup.Update(ref state);
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var delta = SystemAPI.Time.DeltaTime;

        var job = new MovementJob
        {
            deltaTime = delta,
            destPoint = clickedPosition,
            worldTransformLookup = m_WorldTransformLookup
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
    //[ReadOnly] public ComponentLookup<WorldTransform> WorldTransformLookup;
    //public EntityCommandBuffer.ParallelWriter ECB;

    //[BurstCompile]
    //public void Execute([ChunkIndexInQuery] int chunkIndex, ref MovementAspect aspect)
    //{
    //    Debug.Log("MovementJob Execute");
    //    float3 dir = aspect.WorldPosition - destPoint;
    //    aspect.WorldPosition += dir * deltaTime;
    //}

    //[ReadOnly] public float3 destPoint;
    //[BurstCompile]
    //void Execute(ref LocalTransform transform, in MovementComponentData value)
    //{
    //    Debug.Log("MovementJob Execute");
    //    float3 dir = transform.Position - destPoint;
    //    transform.Position += dir * deltaTime;
    //}

    //[ReadOnly] public float3 destPoint;
    //public LocalTransform transform;
    //[BurstCompile]
    //public void Execute()
    //{
    //    Debug.Log("MovementJob Execute");
    //    float3 dir = transform.Position - destPoint;
    //    transform.Position += dir * deltaTime;
    //}

    [ReadOnly] public ComponentLookup<WorldTransform> worldTransformLookup;

    [BurstCompile]
    void Execute(ref MovementAspect aspect)
    {
        Debug.Log("MovementJob Execute");

        var localToWorld = worldTransformLookup[aspect.self];
        var transform = LocalTransform.FromPosition(localToWorld.Position);

        var dir = transform.Position - destPoint;
        transform.Position += dir * deltaTime;
    }
}