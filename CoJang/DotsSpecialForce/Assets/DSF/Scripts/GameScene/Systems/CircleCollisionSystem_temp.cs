using MathExtension;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CircleCollisionSystem_temp : ISystem
{
	EntityQuery chaserQuery;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		using var chaserQueryBuilder = new EntityQueryBuilder(Allocator.TempJob)
		.WithAll<ChaserTag>();

		chaserQuery = state.GetEntityQuery(chaserQueryBuilder);
		state.RequireForUpdate(chaserQuery);
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
		//var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var chaserEntity = chaserQuery.ToEntityArray(Allocator.TempJob);

		var job = new CircleCollisionJob_temp
		{
			boundary = 1.5f,
			entities = chaserEntity,
			otherPositions = state.GetComponentLookup<LocalTransform>(true),
		};

		state.Dependency = job.ScheduleParallel(chaserQuery, state.Dependency);
	}
}

public partial struct CircleCollisionJob_temp : IJobEntity
{
	[ReadOnly] public float boundary;
	[ReadOnly] public float deltaTime;

	[ReadOnly] public NativeArray<Entity> entities;
	[ReadOnly] public ComponentLookup<LocalTransform> otherPositions;

	void Execute(ref LocalTransform transform, in ChaserTag tag)
	{
		foreach (var entity in entities)
		{
			var position = otherPositions[entity].Position;

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
