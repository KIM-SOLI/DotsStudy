using Unity.Entities;
using Unity.Mathematics;

public struct AttackToTarget : IComponentData
{
    public Entity targetEntity;
    public float3 targetPosition;
}