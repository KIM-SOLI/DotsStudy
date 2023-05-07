using System;
using System.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.

[BurstCompile]
partial class DuplicateSkillSystem : SystemBase
{
    EntityQuery soldierQuery;
    private EntityManager entityManager;

    [BurstCompile]
    protected override void OnCreate()
    {
        entityManager = World.EntityManager;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();

        int count = 0;
        float3 spawnPosition = float3.zero;
        // 이동할 엔티티에 MoveToMousePosition 컴포넌트 추가
        Entities.WithAll<MySoldierTag, LifeStateTag>().ForEach((Entity entity, ref LifeStateTag life, ref LocalTransform transform) =>
        {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Z))
            {
                if (life.level >= 3)
                {
                    life.level = 1;
                    count += (int)transform.Scale;
                    spawnPosition = transform.Position;
                    transform.Scale = 1;
                }
            }
        }).Run();

        for(int i=0;i<count; i++)
        {
            // 특정 게임 오브젝트 엔티티 생성
            var newEntity = entityManager.Instantiate(config.TankPrefab);
            entityManager.SetComponentData(newEntity, new LocalTransform { Position = spawnPosition,
                                                                           Scale = 1f});
            entityManager.AddComponent<MySoldierTag>(newEntity);
            entityManager.AddComponent<MoveToTarget>(newEntity);
            //entityManager.SetComponentData<LocalTransform>(newEntity,new LocalTransform { Position = spawnPosition });
        }
    }
}