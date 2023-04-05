﻿using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;


namespace Sample1
{

    public class TeamUnitAuthoring : UnityEngine.MonoBehaviour
    {
        public float Range = 1.5f;
        public float Speed = 0.01f;
        public class TeamUnitBaker : Baker<TeamUnitAuthoring>
        {
            public override void Bake(TeamUnitAuthoring authoring)
            {
                AddComponent(new TeamUnitComponentData { });
                AddComponent(new EnemyTargetComponentData { });
                AddComponent(new RangedWeaponComponentData {Range = authoring.Range });
                AddComponent(new MovableUnitComponentData {moveSpeed = authoring.Speed });
            }
        }
    }


    public struct TeamUnitComponentData : IComponentData
    {
        public int TeamIndex;
    }

    public struct RangedWeaponComponentData : IComponentData
    {
        public float Range;
    }

    public struct MovableUnitComponentData : IComponentData
    {
        public float moveSpeed;
    }

    public struct EnemyTargetComponentData : IComponentData, IEnableableComponent
    {
        public Entity target;
        public float3 targetPosition;
    }

    public readonly partial struct TeamUnitAspect : IAspect
    {
        readonly RefRW<TeamUnitComponentData> _unit;
        readonly RefRW<LocalToWorld> Transform;
        readonly RefRW<LocalTransform> localTransform;

        public readonly Entity Self;

        public int TeamIndex
        {
            get => _unit.ValueRO.TeamIndex;
        }

        public float3 WorldPosition
        {
            get => Transform.ValueRW.Position;
        }

        public void AddWorldPosition(float3 point)
        {
            var origin = Transform.ValueRW.Value;
            var next = origin.Translation() + point;
            Transform.ValueRW.Value = float4x4.TRS(next,
                origin.Rotation(),
                origin.Scale());
        }

    }


    public readonly partial struct RangedWeaponUnitAspect : IAspect
    {
        readonly RefRW<LocalTransform> Transform;
        readonly RefRO<RangedWeaponComponentData> rangedWeapon;
        readonly RefRO<MovableUnitComponentData> move;
        readonly RefRW<EnemyTargetComponentData> target;

        public float Range => rangedWeapon.ValueRO.Range;
        public float Speed => move.ValueRO.moveSpeed;
        public Entity targetEntity => target.ValueRO.target;

        public float3 targetPosition
        {
            get => target.ValueRO.targetPosition;
            set => target.ValueRW.targetPosition = value;
        }

        public float3 WorldPosition
        {
            get => Transform.ValueRW.Position;
            set => Transform.ValueRW.Position = value;
        }

    }
}
