using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitSpawnAuthoring : MonoBehaviour
{
    [SerializeField] public GameObject unitPrefab;

    public class UnitSpawnBaker : Baker<UnitSpawnAuthoring>
    {
        public override void Bake(UnitSpawnAuthoring authoring)
        {
            AddComponent(new SpawnComponent
            {
                unitPrefab = GetEntity(authoring.unitPrefab)
            });
        }
    }
}

public struct SpawnComponent : IComponentData
{
    public Entity unitPrefab;
}

public readonly partial struct SpawnAspect : IAspect
{
    public readonly Entity self;

    private readonly RefRO<SpawnComponent> spawnComponent;
}
