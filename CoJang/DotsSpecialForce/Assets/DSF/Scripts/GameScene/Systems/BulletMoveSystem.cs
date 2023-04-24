using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct BulletMoveSystem : ISystem
{
	private EntityQuery bulletQuery;

	public void OnCreate(ref SystemState state)
	{
		var bulletQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<BulletTag>();
		bulletQuery = state.GetEntityQuery(bulletQueryBuilder);
		state.RequireForUpdate(bulletQuery);

		bulletQueryBuilder.Dispose();
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		var deltaTime = SystemAPI.Time.DeltaTime;

		var bulletMovementJob = new BulletMovementJob
		{
			DeltaTime = deltaTime,
		};
		bulletMovementJob.ScheduleParallel();
	}
}

public partial struct BulletMovementJob : IJobEntity
{
	[ReadOnly] public float DeltaTime;

	void Execute(ref LocalTransform transform, in BulletComponent bulletData)
	{
		transform.Position += transform.Forward() * (bulletData.BulletSpeed * DeltaTime);
	}
}
