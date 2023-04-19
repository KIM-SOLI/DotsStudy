using MathExtension;
using System;
using System.Linq;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

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
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

		var job = new CircleCollisionJob_temp
		{
			boundary = 1.5f,
			localTransformType = state.GetComponentTypeHandle<LocalTransform>(true),

			ecb = ecb.AsParallelWriter(),
			entityhandle = state.GetEntityTypeHandle(),
		};

		state.Dependency = job.ScheduleParallel(chaserQuery, state.Dependency);
	}
}

public partial struct CircleCollisionJob_temp : IJobChunk
{
	[ReadOnly] public float boundary;
	[ReadOnly] public float deltaTime;

	[ReadOnly] public EntityTypeHandle entityhandle;
	[ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformType;

	public EntityCommandBuffer.ParallelWriter ecb;

	public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
	{
		var entities = chunk.GetNativeArray(entityhandle);
		var transforms = chunk.GetNativeArray(ref localTransformType);

		for (int i = 0; i < entities.Length; i++)
		{
			var entity = entities[i];
			var transform = transforms[i];

			for (int j = 0; j < entities.Length; j++)
			{
				if (i == j) // 본인을 제외
					continue;

				var otherTransform = transforms[j];
				var position = otherTransform.Position;

				float distance = math.distancesq(transform.Position, position);
				if (distance <= boundary)
				{
					// Calculate the push direction based on the current positions
					float3 pushDirection = math.normalize(transform.Position - position);

					// Calculate the push distance based on the boundary and current distance
					float pushDistance = (boundary - distance) * 0.5f;

					ecb.SetComponent<LocalTransform>(unfilteredChunkIndex, entity, new LocalTransform
					{
						Position = transform.Position + pushDirection * pushDistance,
						Rotation = transform.Rotation,
						Scale = transform.Scale,
					});
				}
			}
		}
	}
}
