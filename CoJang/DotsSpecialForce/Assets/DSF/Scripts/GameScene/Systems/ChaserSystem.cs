using MathExtension;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ChaserSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        if (playerEntity != null)
        {
            var delta = SystemAPI.Time.DeltaTime;
            Vector3 playerPosition = SystemAPI.GetComponent<WorldTransform>(playerEntity).Position;

            var chaseJob = new ChaserJob
            {
                boundary = 2.0f,
                deltaTime = delta,
                targetPosition = playerPosition
            };
            chaseJob.ScheduleParallel();
        }
    }
}

public partial struct ChaserJob : IJobEntity
{
    [ReadOnly] public float boundary;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float3 targetPosition;

    void Execute(ref TransformAspect transform, in ChaserTag tag)
    {
        if (math.distancesq(transform.LocalPosition, targetPosition) <= boundary)
        {
            Debug.Log("I Got it!");
        }
        else
        {
            var dir = (targetPosition - transform.LocalPosition).Normalize();
            transform.LocalPosition += dir * deltaTime * 1.5f;
        }

        transform.LookAt(targetPosition.float3_XNZ());
    }
}

public partial struct CircleCollisionJob : IJobEntity
{
    [ReadOnly] public float boundary;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public NativeArray<WorldTransform> OtherTransforms;

    void Execute(ref TransformAspect transform, in ChaserTag tag)
    {

    }
}
