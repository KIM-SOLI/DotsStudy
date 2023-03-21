using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementAuthoring : UnityEngine.MonoBehaviour
{
    public float MovementSpeed = 3.5f;

    public class MovementBaker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            AddComponent(new MovementComponentData
            {
                movementSpeed = authoring.MovementSpeed
            });
        }
    }
}


public struct MovementComponentData : IComponentData
{
    public float movementSpeed;
}

public readonly partial struct MovementAspect : IAspect
{
    public readonly Entity self;

    readonly RefRO<MovementComponentData> compData;
    readonly TransformAspect transform;

    public float3 LocalPosition
    {
        get => transform.LocalPosition;
        set => transform.LocalPosition = value;
    }

    public float3 WorldPosition
    {
        get => transform.WorldPosition;
        set => transform.WorldPosition = value;
    }

    public void MoveForward(float deltaTime)
    {
        transform.LocalPosition += transform.Forward * deltaTime;
    }
}