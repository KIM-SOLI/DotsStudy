using Sample1;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Scenes;


namespace Sampel1
{
	
    [DisableAutoCreation]
	[BurstCompile]
	public partial struct FindEnemyTargetSystem : ISystem
	{
		EntityQuery unitQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			//using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
			//	//.WithNone<FindT>()
			//	.WithAllRW<LocalToWorld>();
			//unitQuery = state.GetEntityQuery(unitQueryBuilder);
			//state.RequireForUpdate(unitQuery);
			SubScene scene;
        }

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			//var job = new FindEnemyTargetJob { };
			//job.ScheduleParallel();
		}
	}


	//[BurstCompile]
	//public partial struct FindEnemyTargetJob : IJobEntity
	//{
	//	public float Delta;

	//	void Execute(ref LocalTransform transform, in FindEnemyTargetComponentData value)
	//	{
	//	}
	//} 
}