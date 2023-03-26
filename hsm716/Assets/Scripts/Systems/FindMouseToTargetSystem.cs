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
        // 메인 카메라 가져오기
        mainCamera = Camera.main;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        float3 targetPosition = float3.zero;

        // 마우스 위치로 Ray를 쏴서 충돌 지점 계산
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 계산된 충돌
            targetPosition = new float3(hit.point.x, 0, hit.point.z);
        }

        if (!targetPosition.Equals(float3.zero))
        {
            // 이동할 엔티티에 MoveToMousePosition 컴포넌트 추가
            Entities.ForEach((Entity entity, ref MoveToTarget moveComp) =>
            {
                moveComp.targetPosition = targetPosition;
                moveComp.moveSpeed = 5f;
            }).Run();
        }

    }
}
