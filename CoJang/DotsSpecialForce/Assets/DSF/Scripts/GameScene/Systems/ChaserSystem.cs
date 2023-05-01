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
        var delta = SystemAPI.Time.DeltaTime;
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        if (playerEntity != null)
        {
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
        var dir = (targetPosition - transform.LocalPosition).Normalize();

        if (math.distancesq(transform.LocalPosition, targetPosition) <= boundary)
        {
            // Debug.Log("I Got it!");
            // transform.LocalPosition = targetPosition + (-dir * boundary); // 밀어내기 테스트
        }
        else
        {
            transform.LocalPosition += dir * deltaTime * 1.5f;
        }

        transform.LookAt(targetPosition.float3_XNZ());
    }
}


