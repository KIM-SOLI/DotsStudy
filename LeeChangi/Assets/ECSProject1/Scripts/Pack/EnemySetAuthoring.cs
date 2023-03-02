using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using System.Numerics;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct EnemySetData
{
	public UnityEngine.Vector3 position;
    public GameObject prefab;
}

public class EnemySetAuthoring : UnityEngine.MonoBehaviour
{
	public List<EnemySetData> dataSet;
    public class EnemySetBaker : Baker<EnemySetAuthoring>
    {
        public override void Bake(EnemySetAuthoring authoring)
        {
            AddComponent(new EnemySetComponentData { });
        }
    }
}


struct EnemySetComponentData : IComponentData
{
}


[BurstCompile]
public partial struct EnemySetSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<EnemySetComponentData>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var job = new EnemySetJob { };
		job.Schedule();
	}
}


[BurstCompile]
public partial struct EnemySetJob : IJobEntity
{
	void Execute(ref LocalTransform transform, in EnemySetComponentData value)
	{
	}
}