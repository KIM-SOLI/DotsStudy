using Assets.Scripts.ComponentsAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSpawner : MonoBehaviour, Actions.IPlayerActionsActions
{
    private EntityManager _entityManager;
    private EntityQuery _characterQuery;

    [SerializeField] Camera cam;

    Actions action;
    Vector2 position = Vector2.zero;
    Plane groundPlane = new Plane(Vector3.up, 0);
    bool isSpawnedCharacter = false;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _characterQuery = _entityManager.CreateEntityQuery(typeof(CharacterHeading));
    }


    void Awake()
    {
        action = new Actions();
        action.PlayerActions.SetCallbacks(this);
    }

    private void OnEnable()
    {
        action.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        action.PlayerActions.Disable();
    }


    void Update()
    {
    }

    public void OnMoveOrSpawn(InputAction.CallbackContext context)
    {
        var ray = cam.ScreenPointToRay(position);
        if (groundPlane.Raycast(ray, out var enter))
        {
            var clickPos = ray.GetPoint(enter);

            if (isSpawnedCharacter == false)
            {
                SpwanCharacter(clickPos, enter);
                isSpawnedCharacter = true;
            }
            else
            {
                ChangeHead(clickPos, enter);
            }
        }

    }

    public void SpwanCharacter(Vector3 clickPos, float enter)
    {
        var query = _entityManager.CreateEntityQuery(typeof(SettingComponentData));
        var setting = query.GetSingleton<SettingComponentData>();
        var entity = _entityManager.Instantiate(setting.targetEntity);
        var pos = new UniformScaleTransform { Position = clickPos, Scale = 1 };
        _entityManager.SetComponentData(entity, new LocalToWorldTransform { Value = pos });

        var headingPos = new UniformScaleTransform { Position = enter + pos.Forward(), Scale = 1 };
        var characterHeading = MathHelpers.GetHeading(pos.Position, headingPos.Position);
        _entityManager.AddComponentData(entity, new CharacterHeading { Value = characterHeading });
    }

    public void ChangeHead(Vector3 clickPos, float enter)
    {
        var entities = _characterQuery.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entities)
        {
            var entityTransform = _entityManager.GetComponentData<LocalToWorldTransform>(entity);
            var entityPos = entityTransform.Value.Position;

            var characterHeading = MathHelpers.GetHeading(entityPos, clickPos);
            _entityManager.SetComponentData(entity, new CharacterHeading { Value = characterHeading });
        }

        entities.Dispose();
    }

    public void OnClickPosition(InputAction.CallbackContext context)
    {
        position = context.ReadValue<Vector2>();
    }
}
