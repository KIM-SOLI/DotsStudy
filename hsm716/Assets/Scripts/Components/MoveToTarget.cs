using Unity.Entities;
using Unity.Mathematics;

public struct MoveToTarget : IComponentData
{
    public float3 targetPosition;
    public float moveSpeed;
}