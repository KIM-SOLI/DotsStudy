using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;

    public float defaultBulletSpeed = 10.0f;
    public float defaultBulletRange = 15.0f;
    public int defaultBulletDamage = 1;
    public int defaultPanetrateNum = 1;

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            AddComponent(new BulletComponent
            {
                bullet = GetEntity(authoring.bulletPrefab),
            });

            AddComponent(new BulletTag
            {
                BulletDamage = authoring.defaultBulletDamage,
                BulletRange = authoring.defaultBulletRange,
                BulletSpeed = authoring.defaultBulletSpeed,
                PanetrateNum = authoring.defaultPanetrateNum,
            });

            AddComponent(new DestoryTag
            {
                IsDestoryed = false,
            });
        }
    }
}

public struct BulletTag : IComponentData
{
    public Entity self;

    public float BulletSpeed;
    public float BulletRange;
    public int BulletDamage;

    public int PanetrateNum;
    public float3 spawnedPosition;
}

public struct BulletComponent : IComponentData
{
    public Entity bullet;
}
