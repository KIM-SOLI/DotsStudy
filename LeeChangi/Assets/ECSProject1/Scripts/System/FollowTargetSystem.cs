using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[DisableAutoCreation]
[BurstCompile]
public partial struct FollowTargetSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		//var job = new FollowTargetJob { };
		//job.ScheduleParallel();
	}
}


[BurstCompile]
public partial struct FollowTargetJob : IJobEntity
{
	public float Delta;

	void Execute(ref LocalTransform transform, in TargetingEnemyUnitComponentData value)
	{
		
	}
}


