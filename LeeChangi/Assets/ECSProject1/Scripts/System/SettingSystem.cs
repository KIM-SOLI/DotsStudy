using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
//using Unity.VisualScripting.FullSerializer;


[BurstCompile]
public partial struct SettingSystem : ISystem
{
    void Spawn(ref SystemState state, Entity prefab, int count, ref Random random)
    {

        var units = state.EntityManager.Instantiate(prefab, count, Allocator.Temp);
        for (var i = 0; i < units.Length; i++)
        {
            var position = new float3();
            position.xz = random.NextFloat2() * 200 - 100;
            state.EntityManager.SetComponentData(units[i],
                new LocalTransform { Position = position, Scale = 1 });
            //state.EntityManager.SetComponentData(units[i],
            //    new MovementComponentData{ });
        }

    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SettingComponentData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = Random.CreateFromIndex(1443);
        var setting = SystemAPI.GetSingleton<SettingComponentData>();
        //Spawn(ref state, setting.unitEntity, setting.unitCount, ref random);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var vehicles = CollectionHelper.CreateNativeArray<Entity>(setting.unitCount, Allocator.Temp);
        ecb.Instantiate(setting.unitEntity, vehicles);

        state.Enabled = false;

    }
}


//[BurstCompile]
//public partial struct SettingJob : IJobEntity
//{
//	public float Delta;

//	void Execute(in SettingComponentData value)
//	{

//	}
//}