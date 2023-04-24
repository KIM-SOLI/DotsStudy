using MathExtension;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
	public float MovementSpeed = 10.0f;

	public class MovementBaker : Baker<MovementAuthoring>
	{
		public override void Bake(MovementAuthoring authoring)
		{
			AddComponent(new MovementComponentData
			{
				movementSpeed = authoring.MovementSpeed
			});

			AddComponent<PlayerTag>();
		}
	}
}


public struct PlayerTag : IComponentData
{

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


	public void MoveToPointOnlyXZ(float3 destPoint, float deltaTime)
	{
		float3 dir = (destPoint - LocalPosition).Normalize();
		dir *= compData.ValueRO.movementSpeed * deltaTime;

		transform.LookAt(destPoint.float3_XNZ());
		transform.LocalPosition += dir.float3_XNZ();
	}
}