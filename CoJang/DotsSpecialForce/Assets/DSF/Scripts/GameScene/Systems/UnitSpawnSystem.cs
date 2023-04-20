using MathExtension;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public partial struct UnitSpawnSystem : ISystem
{
	private NativeArray<Vector3> clickedPosition;
	private float intervalTime;
	private float currentTime;

	private int spawnCount;
	private int spawnLimit;

	private EntityQuery spawnQuery;

	[BurstDiscard]
	public void OnCreate(ref SystemState state)
	{
		spawnLimit = 500;
		spawnCount = 0;
		currentTime = 0.0f;
		intervalTime = 0.01f;
		clickedPosition = new NativeArray<Vector3>(1, Allocator.Persistent);

		var spawnQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<SpawnComponent>();
		spawnQuery = state.GetEntityQuery(spawnQueryBuilder);
		state.RequireForUpdate(spawnQuery);


		InputSystem.inputActionMap.Disable();

		var action = InputSystem.inputActionMap.AddAction("CtrlClick");
		action.AddCompositeBinding("OneModifier").
			With("Binding", "<Mouse>/leftButton").
			With("Modifier", "<KeyBoard>/leftCtrl");

		action.performed += OnCtrlClick;

		InputSystem.inputActionMap.Enable();
	}

	[BurstDiscard]
	public void OnDestroy(ref SystemState state)
	{
		var action = InputSystem.inputActionMap.FindAction("CtrlClick");
		if (action != null)
		{
			action.performed -= OnCtrlClick;
		}

		clickedPosition.Dispose();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		if (spawnCount < spawnLimit)
		{
			currentTime += SystemAPI.Time.DeltaTime;
			if (currentTime > intervalTime)
			{
				var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
				var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

				var setting = spawnQuery.GetSingleton<SpawnComponent>();
				var entity = ecb.Instantiate(setting.unitPrefab);

				ecb.AddComponent<ChaserTag>(entity);
				ecb.SetComponent(entity, new LocalTransform { Position = float3.zero, Scale = 0.8f });

				currentTime = 0.0f;
				spawnCount++;
			}
		}
	}

	[BurstDiscard]
	private void OnCtrlClick(InputAction.CallbackContext context)
	{
		Debug.Log("Control + Left-Click");

		if (Raycaster.ShootRay())
		{
			clickedPosition[0] = Raycaster.Hit.point.Vector3_XNZ();

			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			var spawnQuery = entityManager.CreateEntityQuery(typeof(SpawnComponent));
			var setting = spawnQuery.GetSingleton<SpawnComponent>();
			var entity = entityManager.Instantiate(setting.unitPrefab);

			entityManager.SetComponentData(entity, new LocalTransform { Position = clickedPosition[0], Scale = 1.5f });
			entityManager.AddComponent<ChaserTag>(entity);
		}
	}
}