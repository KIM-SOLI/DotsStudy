using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace Sample1
{


	public class CheckBodyStatSystemAuthoring : IGetBakedSystem
	{
		public CheckBodyStatSystemAuthoring(){}
		public Type GetSystemType()
		{
			return typeof(CheckBodyStatSystem);
		}
	}
		
	[DisableAutoCreation]
	[BurstCompile]
	public partial struct CheckBodyStatSystem : ISystem
	{
        EntityQuery unitQuery;

        [BurstCompile]
		public void OnCreate(ref SystemState state)
		{
            using var targetterQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
               .WithAll<BodyStat>();

			unitQuery = state.GetEntityQuery(targetterQueryBuilder);
            state.RequireForUpdate(unitQuery);
        }

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			//var job = new CheckBodyStatJob { };
			//job.ScheduleParallel();
		}
	}

    [BurstCompile]
    public partial struct CheckBodyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref BodyStat bodyStat)
        {
			Ecb.SetComponent(chunkIndex, bodyStat.hpStateEntity, new URPMaterialPropertyBaseColor
			{
				//Value = new 
			});
        }
    }
}