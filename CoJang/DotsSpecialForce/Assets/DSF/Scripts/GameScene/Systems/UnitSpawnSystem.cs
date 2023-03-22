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
    [BurstDiscard]
    public void OnCreate(ref SystemState state)
    {
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
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    }

    [BurstDiscard]
    private void OnCtrlClick(InputAction.CallbackContext context)
    {
        Debug.Log("Control + Left-Click");

        if (Raycaster.ShootRay())
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var unitQuery = entityManager.CreateEntityQuery(typeof(MovementComponentData));

            var spawnQuery = entityManager.CreateEntityQuery(typeof(SpawnComponent));
            var setting = spawnQuery.GetSingleton<SpawnComponent>();
            var entity = entityManager.Instantiate(setting.unitPrefab);

            entityManager.SetComponentData(entity, new LocalTransform { Position = Raycaster.Hit.point });
        }
    }
}

public partial struct SpawnJob : IJobEntity
{

}
