using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.UIElements;



[BurstCompile]
public partial struct MovementSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<MovementComponentData>();
		
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
    {

   //     var random = Random.CreateFromIndex(1234);
   //     var job = new MovementJob { 
			//dir = random.NextFloat2(),
			//deltaTime = SystemAPI.Time.DeltaTime,
   //     };
   //     job.ScheduleParallel();
    }
}


[BurstCompile]
public partial struct MovementJob : IJobEntity
{
	[ReadOnly]public float2 dir;
	[ReadOnly] public float deltaTime;
	
	
	void Execute(ref LocalTransform transform, in MovementComponentData value)
	{
		const float maxDistance = 5;
		transform.Position.xz += dir* deltaTime;
        if(transform.Position.x > maxDistance) { transform.Position.x = maxDistance; }
        if (transform.Position.x < -maxDistance) { transform.Position.x = -maxDistance; }
        if (transform.Position.z < -maxDistance) { transform.Position.x = -maxDistance; }
        if (transform.Position.z < maxDistance) { transform.Position.x = maxDistance; }

    }
}