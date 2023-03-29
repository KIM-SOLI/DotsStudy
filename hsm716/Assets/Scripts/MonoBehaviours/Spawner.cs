using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities.Hybrid;
using Random = UnityEngine.Random;

/*

public class Spawner : MonoBehaviour
{

    class SpawnerBaker : Baker<Spawner>
    {
        public override void Bake(Spawner authoring)
        {

        }
    }

    [Header("Spawner")]
    // number of enemies generated per interval
    [SerializeField] private int spawnCount = 30;

    // time between spawns
    [SerializeField] private float spawnInterval = 3f;

    // enemies spawned on a circle of this radius
    [SerializeField] private float spawnRadius = 30f;

    // extra enemy increase each wave
    [SerializeField] private int difficultyBonus = 5;

    [Header("Enemy")]
    // random speed range
    [SerializeField] float minSpeed = 4f;
    [SerializeField] float maxSpeed = 12f;

    // counter
    private float spawnTimer;

    // flag from GameManager to enable spawning
    private bool canSpawn;
    private EntityManager entityManager;


    [SerializeField] private Mesh enemyMesh;
    [SerializeField] private Material enemyMaterial;

    [SerializeField] private GameObject enemyPrefab;

    private Entity enemyEntityPrefab;

    private void Start()
    {
        // Entity ¸Å´ÏÀú;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //var conversionSettings = GameObjectConversionUtility.ConvertSettingsForConversionWorld(World.DefaultGameObjectInjectionWorld);
        //var enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, conversionSettings);
        //entityManager.Instantiate(enemyEntityPrefab);
    }

    // spawns enemies in a ring around the player
    private void SpawnWave()
    {
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);

        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);
            //entityManager.SetComponentData(enemyArray[i], new Translation { Value = RandomPointOnCircle(spawnRadius) });
            entityManager.SetComponentData(enemyArray[i], new MoveToTarget { moveSpeed = Random.Range(minSpeed, maxSpeed) });
        }

        enemyArray.Dispose();
        spawnCount += difficultyBonus;
    }

    // get a random point on a circle with given radius
    private float3 RandomPointOnCircle(float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;

        // return random point on circle, centered around the player position
        return new float3(randomPoint.x, 0.5f, randomPoint.y);
    }

    // signal from GameManager to begin spawning
    public void StartSpawn()
    {
        canSpawn = true;
    }

    private void Update()
    {
        // count up until next spawn
        /*spawnTimer += Time.deltaTime;

        // spawn and reset timer
        if (spawnTimer > spawnInterval)
        {
            SpawnWave();
            spawnTimer = 0;
        }
    }
}
*/