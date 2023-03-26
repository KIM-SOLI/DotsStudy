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

    [BurstDiscard]
    public void OnCreate(ref SystemState state)
    {
        clickedPosition = new NativeArray<Vector3>(1, Allocator.Persistent);

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
        var delta = SystemAPI.Time.DeltaTime;

        var chaseJob = new ChaserJob
        {
            deltaTime = delta,
            targetPosition = float3.zero,
        };
        chaseJob.Schedule();
    }

    [BurstDiscard]
    private void OnCtrlClick(InputAction.CallbackContext context)
    {
        Debug.Log("Control + Left-Click");

        if (Raycaster.ShootRay())
        {
            clickedPosition[0] = new Vector3(Raycaster.Hit.point.x, 0, Raycaster.Hit.point.z);

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var spawnQuery = entityManager.CreateEntityQuery(typeof(SpawnComponent));
            var setting = spawnQuery.GetSingleton<SpawnComponent>();
            var entity = entityManager.Instantiate(setting.unitPrefab);

            entityManager.SetComponentData(entity, new LocalTransform { Position = clickedPosition[0], Scale = 1.5f });
            entityManager.AddComponent<ChaserTag>(entity);
        }
    }
}

public struct ChaserTag : IComponentData
{

}

public partial struct ChaserJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float3 targetPosition;

    void Execute(ref LocalTransform transform, in ChaserTag tag)
    {
        var dir = targetPosition - transform.Position;
        transform.Position += dir * deltaTime;
    }
}
