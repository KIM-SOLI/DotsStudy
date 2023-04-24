using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text countText;

    private EntityManager entityManager;
    private Entity sqwanEntity;

    void OnDisable()
    {

    }

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        sqwanEntity = entityManager.CreateEntityQuery(typeof(EnemyCount)).GetSingletonEntity();
    }

    // Update is called once per frame
    void Update()
    {
        countText.text = $"{entityManager.GetComponentData<EnemyCount>(sqwanEntity).enemyCount}";
    }
}
