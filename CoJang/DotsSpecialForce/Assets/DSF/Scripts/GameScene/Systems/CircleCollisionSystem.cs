using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct CircleCollisionSystem : ISystem
{
	EntityQuery chaserQuery;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		var chaserQueryBuilder = new EntityQueryBuilder(Allocator.TempJob)
		.WithAll<ChaserTag>();

		chaserQuery = state.GetEntityQuery(chaserQueryBuilder);
		state.RequireForUpdate(chaserQuery);

		chaserQueryBuilder.Dispose();
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
		var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
		var entityArray = chaserQuery.ToEntityArray(Allocator.TempJob);

		var job = new CircleCollisionJob
		{
			boundary = 1.5f,
			localTransformType = state.GetComponentTypeHandle<LocalTransform>(true),

			ecb = ecb.AsParallelWriter(),
			entityhandle = state.GetEntityTypeHandle(),

			otherEntities = entityArray,
			otherTransforms = state.GetComponentLookup<LocalTransform>(true),
		};

		state.Dependency = job.ScheduleParallel(chaserQuery, state.Dependency);
	}
}

public partial struct CircleCollisionJob : IJobChunk
{
	[ReadOnly] public float boundary;
	[ReadOnly] public float deltaTime;

	[ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformType;
	[ReadOnly] public ComponentLookup<LocalTransform> otherTransforms;

	[ReadOnly] public NativeArray<Entity> otherEntities;

	[ReadOnly] public EntityTypeHandle entityhandle;
	public EntityCommandBuffer.ParallelWriter ecb;

	public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
	{
		var entities = chunk.GetNativeArray(entityhandle);
		var transforms = chunk.GetNativeArray(ref localTransformType);

		for (int i = 0; i < entities.Length; i++) // Chunk 내 Entity들
		{
			var entity = entities[i];
			var transform = transforms[i];

			for (int j = 0; j < otherEntities.Length; j++) // 다른 모든(Query 내의) Entity들
			{
				var otherEntity = otherEntities[j];

				if (entity == otherEntity) // 본인을 제외
					continue;

				var position = otherTransforms[otherEntity].Position;

				float distance = math.distancesq(transform.Position, position);
				if (distance <= boundary)
				{
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
