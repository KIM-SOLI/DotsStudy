using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using System.ComponentModel;

[BurstCompile]
public partial struct FollowEnemySystem : ISystem
{
	EntityQuery unitQuery;
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		unitQuery = SystemAPI.QueryBuilder().WithAll<TeamUnitComponentData>().Build();
		state.RequireForUpdate(unitQuery);
    }

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		//var entities = unitQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
		
		//var job = new FollowEnemyJob { };
		//job.ScheduleParallel();
	}
}


//[BurstCompile]
//public partial struct FollowEnemyJob : IJobEntity
//{
//	//[ReadOnly] NativeArray<WorldTransform>

//	void Execute(ref LocalTransform transform, in TeamUnitAspect value)
//	{
//	}
//}