using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEditor.PlayerSettings;


[BurstCompile]
partial class FindMouseToTargetSystem : SystemBase
{
    private Camera mainCamera;
    private EntityManager entityManager;
    Vector3 randomDir;

    [BurstCompile]
    protected override void OnCreate()
    {
        // ���� ī�޶� ��������
        mainCamera = Camera.main;

        randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;

        entityManager = World.EntityManager;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        float3 targetPosition = float3.zero;

        // ���콺 ��ġ�� Ray�� ���� �浹 ���� ���
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ���� �浹
            targetPosition = new float3(hit.point.x, 0, hit.point.z);
        }

        

        
        // �̵��� ��ƼƼ�� MoveToMousePosition ������Ʈ �߰�
        Entities.WithAll<MoveToTarget, MySoldierTag>().ForEach((Entity entity, ref MoveToTarget moveComp) =>
        {
            if (!targetPosition.Equals(float3.zero))
            {
                moveComp.targetPosition = targetPosition;
            }
            moveComp.moveSpeed = 5f;
        }).Run();


        Entities.WithAll<AttackToTarget, MySoldierTag>().ForEach((Entity entity, ref AttackToTarget attackComp) =>
        {
            var soldierEntityLocalTransform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 soldierPosition = soldierEntityLocalTransform.Position;
            float minDist = 100000000f;
            float3 closestEnemyPosition = float3.zero;
            float targetScale = 1f;
            // ���� ��ȸ�ϸ� ���� ����� ���� ã
            var enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyTag));
            var entityArray = enemyQuery.ToEntityArray(Allocator.TempJob);
            Entity tempEntity = Entity.Null;
            foreach (var enemyEntity in entityArray)
            {
                var localTransform = entityManager.GetComponentData<LocalTransform>(enemyEntity);
                var enemyPosition = localTransform.Position;
                float dist = math.distance(soldierPosition, enemyPosition);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemyPosition = enemyPosition;
                    tempEntity = enemyEntity;
                    targetScale = localTransform.Scale;
                }
            }
            entityArray.Dispose();
            attackComp.targetEntity = tempEntity;
            attackComp.targetPosition = closestEnemyPosition;
            attackComp.targetScale = targetScale;
        }).WithoutBurst().Run();



        Entities.WithAll<MoveToTarget, EnemyTag>().ForEach((Entity entity, ref MoveToTarget moveComp,ref EnemyTag enemy) =>
        {
            enemy.dirChangeTime += SystemAPI.Time.DeltaTime;
            moveComp.moveSpeed = 3f;
            if (enemy.dirChangeTime > 3f)
            {
                moveComp.targetPosition = new float3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                enemy.dirChangeTime = 0;
            }
        }).Run();

    }
}
