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

    public void OnUpdate(ref SystemState state)
    {
        var delta = SystemAPI.Time.DeltaTime;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        if (playerEntity != null)
        {
            Vector3 playerPosition = SystemAPI.GetComponent<WorldTransform>(playerEntity).Position;

            var chaseJob = new ChaserJob
            {
                deltaTime = delta,
                targetPosition = playerPosition
            };
            chaseJob.ScheduleParallel();
        }
    }
}

public partial struct ChaserJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float3 targetPosition;

    void Execute(ref TransformAspect transform, in ChaserTag tag)
    {
        var dir = targetPosition - transform.LocalPosition;

        // extension¿∏∑Œ ª¨ ∞Õ
        var tempDir = new Vector3(dir.x, 0, dir.z);
        tempDir.Normalize();

        dir = tempDir;

        transform.LookAt(new float3(targetPosition.x, 0, targetPosition.z));
        transform.LocalPosition += dir * deltaTime * 1.5f;
    }
}
