using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
partial class FindMouseToTargetSystem : SystemBase
{
    private Camera mainCamera;

    [BurstCompile]
    protected override void OnCreate()
    {
        // ���� ī�޶� ��������
        mainCamera = Camera.main;
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
            Entities.ForEach((Entity entity, ref MoveToTarget moveComp) =>
            {
                moveComp.targetPosition = targetPosition;
                moveComp.moveSpeed = 5f;
            }).Run();
        }

    }
}
