using MathExtension;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CircleCollisionSystem : ISystem
{
	EntityQuery chaserQuery;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		chaserQuery = state.GetEntityQuery(ComponentType.ReadOnly<ChaserTag>());
		state.RequireForUpdate(chaserQuery);
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		var entityArray = chaserQuery.ToEntityArray(Allocator.TempJob);
		var entityManager = state.World.EntityManager;

		NativeArray<float3> positionList = new NativeArray<float3>(entityArray.Length, Allocator.TempJob);
		for (int i = 0; i < entityArray.Length; i++)
		{
			positionList[i] = entityManager.GetComponentData<LocalTransform>(entityArray[i]).Position;
		}

		var delta = SystemAPI.Time.DeltaTime;
		/*        var collisionJob = new CircleCollisionJob
				{
					boundary = 1.5f,
					deltaTime = delta,
					otherPositions = positionList,
				};

				JobHandle jobHandle = collisionJob.ScheduleParallel(chaserQuery, default);
				jobHandle.Complete();*/

		entityArray.Dispose();
		positionList.Dispose();
	}
}

public partial struct CircleCollisionJob : IJobEntity
{
	[ReadOnly] public float boundary;
	[ReadOnly] public float deltaTime;
	[ReadOnly] public NativeArray<float3> otherPositions;

	void Execute(ref LocalTransform transform, in ChaserTag tag)
	{
		foreach (var position in otherPositions)
		{
			if (!transform.Position.Equals(position)) // 본인을 제외
			{
				if (math.distancesq(transform.Position, position) <= boundary)
				{
					//Debug.Log("CircleCollision");
					var dir = VectorExtension.NormalizedDirAB(transform.Position, position);

					transform.Position += -dir * boundary;
				}
			}
		}
	}
}
