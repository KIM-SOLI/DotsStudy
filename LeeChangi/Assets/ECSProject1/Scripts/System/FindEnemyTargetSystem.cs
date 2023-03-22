using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;


namespace Sampel1
{

	[BurstCompile]
	public partial struct FindEnemyTargetSystem : ISystem
	{
		EntityQuery unitQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			using var unitQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
				//.WithNone<FindT>()
				.WithAllRW<LocalToWorld>();
			unitQuery = state.GetEntityQuery(unitQueryBuilder);
			state.RequireForUpdate(unitQuery);
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