using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine;
using UnityEditor.Graphs;

namespace Sample1
{

    public class TeamUnitAuthoring : UnityEngine.MonoBehaviour
    {
        public float Range = 1f;
        public float LockOnRange = 1.5f;
        public float LockOffRange = 2f;
        public float Speed = 1f;
        public float HP = 100;

        public GameObject hpStateObject;
        public class TeamUnitBaker : Baker<TeamUnitAuthoring>
        {
            public override void Bake(TeamUnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TeamUnitComponentData { });
                AddComponent(entity, new EnemyTargetComponentData { });
                AddComponent(entity, new RangedWeaponComponentData
                {
                    RangeDistanceSq = math.pow(authoring.Range, 2),
                    MaximumDistanceSq = math.pow(authoring.LockOffRange, 2),
                    MinimumDistanceSq = math.pow(authoring.LockOnRange, 2),
                });
                AddComponent(entity, new MovableUnitComponentData { moveSpeed = authoring.Speed });

                AddComponent(entity, new BodyStat { 
                    HP = authoring.HP, 
                    Armor = 0,
                    hpStateEntity = GetEntity(authoring.hpStateObject, TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new LockOnTargetComponentData { });

                SetComponentEnabled<EnemyTargetComponentData>(entity, false);
                SetComponentEnabled<LockOnTargetComponentData>(entity, false);

                
            }
        }
    }


    public struct TeamUnitComponentData : IComponentData
    {
        public int TeamIndex;
    }

    public struct RangedWeaponComponentData : IComponentData
    {
        public float RangeDistanceSq;

        public float MinimumDistanceSq;
        public float MaximumDistanceSq;
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

    public struct BodyStat : IComponentData
    {
        public float HP;
        public float Armor;

        public Entity hpStateEntity;
    }

    public struct LockOnTargetComponentData : IComponentData, IEnableableComponent
    { }

    public struct TestComponent : IComponentData
    {
        public int i;
    }

    public struct HPBar : IComponentData
    {
        public float HP;
    }

    //public readonly partial struct TargetingEnemyUnitAspect : IAspect
    //{
    //    readonly RefRO<LocalToWorld> transform;
    //    public readonly Entity self;
    //    readonly RefRO<TeamUnitComponentData> teamUnit;

    //    public float3 WorldPosition => transform.ValueRO.Position;
    //    public int TeamIndex => teamUnit.ValueRO.TeamIndex;
    //}

}
