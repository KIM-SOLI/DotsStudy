using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
partial class FindMouseToTargetSystem : SystemBase
{
    private Camera mainCamera;

    Vector3 randomDir;

    [BurstCompile]
    protected override void OnCreate()
    {
        // ���� ī�޶� ��������
        mainCamera = Camera.main;

        randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
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

        if (!targetPosition.Equals(float3.zero))
        {
            // �̵��� ��ƼƼ�� MoveToMousePosition ������Ʈ �߰�
            Entities.WithNone<EnemyTag>().ForEach((Entity entity, ref MoveToTarget moveComp) =>
            {
                moveComp.targetPosition = targetPosition;
                moveComp.moveSpeed = 5f;
            }).Run();
        }

        Entities.WithAll<MoveToTarget, EnemyTag>().ForEach((Entity entity, ref MoveToTarget moveComp,ref EnemyTag enemy) =>
        {
            enemy.dirChangeTime += SystemAPI.Time.DeltaTime;
            moveComp.moveSpeed = 3f;
            if (enemy.dirChangeTime > 3f)
            {
                moveComp.targetPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));//.normalized;
                enemy.dirChangeTime = 0;
            }
        }).Run();

    }
}
