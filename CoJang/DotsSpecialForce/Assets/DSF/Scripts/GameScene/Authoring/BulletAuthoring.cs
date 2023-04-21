using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
	[SerializeField] public GameObject bulletPrefab;

	public float defaultBulletSpeed = 10.0f;
	public float defaultBulletRange = 15.0f;
	public int defaultBulletDamage = 1;

	public class BulletBaker : Baker<BulletAuthoring>
	{
		public override void Bake(BulletAuthoring authoring)
		{
			AddComponent(new BulletComponent
			{
				bullet = GetEntity(authoring.bulletPrefab),

				BulletDamage = authoring.defaultBulletDamage,
				BulletRange = authoring.defaultBulletRange,
				BulletSpeed = authoring.defaultBulletSpeed,
			});

			AddComponent<BulletTag>();
		}
	}
}

public struct BulletTag : IComponentData
{

}

public struct BulletComponent : IComponentData
{
	public Entity bullet;

	public float BulletSpeed;
	public float BulletRange;
	public int BulletDamage;
}
