using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GraveyardMono : MonoBehaviour
{
    public float2 FieldDimenstions;
    public int NumberTombstonesToSpawn;
    public GameObject TombstonePrefab;
    public uint RandomSeed;
    public GameObject ZombiePrefab;
    public float ZombieSpawnRate;

    public class GraveyardBaker : Baker<GraveyardMono>
    {
        public override void Bake(GraveyardMono authoring)
        {
            AddComponent(new GraveyardProperties
            {
                FieldDimenstions = authoring.FieldDimenstions,
                NumberTombstonesToSpawn = authoring.NumberTombstonesToSpawn,
                TombstonePrefab = GetEntity(authoring.TombstonePrefab),
                ZombiePrefab = GetEntity(authoring.ZombiePrefab),
                ZombieSpawnRate =  authoring.ZombieSpawnRate
            });
            AddComponent(new GraveyardRandom
            {
                Value = Random.CreateFromIndex(authoring.RandomSeed)
            });
            AddBuffer<ZombieSpawnPoints>();
            AddComponent<ZombieSpawnTimer>();

        }
    }
}