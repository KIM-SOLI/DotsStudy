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

        //var entityQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<WorldTransform>().WithAll<ChaserTag>();
        //var entityQuery = state.GetEntityQuery(entityQueryBuilder);
        //state.RequireForUpdate(entityQuery);

        //var nativeArray =
        //    CollectionHelper.CreateNativeArray<WorldTransform>(entityQuery.CalculateEntityCount(), Allocator.TempJob);

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
            Debug.Log("I Got it!");
            // transform.LocalPosition = targetPosition + (-dir * boundary); // 밀어내기 테스트
        }
        else
        {
            transform.LocalPosition += dir * deltaTime * 1.5f;
        }

        transform.LookAt(targetPosition.float3_XNZ());
    }
}

public partial struct CircleCollisionJob : IJobEntity
{
    [ReadOnly] public float boundary;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public NativeArray<float3> otherPositions;

    void Execute(ref TransformAspect transform, in ChaserTag tag)
    {
        foreach (var position in otherPositions)
        {
            if (math.distancesq(transform.LocalPosition, position) <= boundary)
            {
                var dir = VectorExtension.NormalizedDirAB(transform.LocalPosition, position);
            }
        }
    }
}
