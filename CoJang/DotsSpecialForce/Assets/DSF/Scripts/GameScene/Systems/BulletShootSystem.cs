using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using UnityEngine.InputSystem;
using MathExtension;
using Unity.Transforms;

public partial struct BulletShootSystem : ISystem
{
    EntityQuery bulletQuery;
    private NativeArray<Vector3> clickedPosition;

    [BurstDiscard]
    public void OnCreate(ref SystemState state)
    {
        clickedPosition = new NativeArray<Vector3>(2, Allocator.Persistent);

        var bulletQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<BulletTag>();
        bulletQuery = state.GetEntityQuery(bulletQueryBuilder);

        state.RequireForUpdate(bulletQuery);

        InputSystem.ClickAction.performed += OnClick;
    }

    [BurstDiscard]
    public void OnDestroy(ref SystemState state)
    {
        InputSystem.ClickAction.performed -= OnClick;
        clickedPosition.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        if (playerEntity != null)
        {
            Vector3 playerPosition = SystemAPI.GetComponent<WorldTransform>(playerEntity).Position;
            clickedPosition[1] = playerPosition;
        }
    }

    [BurstDiscard]
    private void OnClick(InputAction.CallbackContext context)
    {
        if (Raycaster.ShootRay())
        {
            clickedPosition[0] = Raycaster.Hit.point.Vector3_XNZ();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var spawnQuery = entityManager.CreateEntityQuery(typeof(BulletComponent));
            var setting = spawnQuery.GetSingleton<BulletComponent>();
            var entity = entityManager.Instantiate(setting.bullet);

            entityManager.AddComponent<BulletTag>(entity);

            var tag = new BulletTag
            {
                BulletSpeed = 20.0f,
                BulletDamage = 1,
                BulletRange = 150.0f,
                PanetrateNum = 1,
                spawnedPosition = clickedPosition[0],

                self = entity,
            };
            entityManager.SetComponentData(entity, tag);

            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            transform.Position = clickedPosition[1];
            transform.Rotation = Quaternion.LookRotation(clickedPosition[0] - clickedPosition[1]);

            entityManager.SetComponentData(entity, transform);
        }
    }
}
