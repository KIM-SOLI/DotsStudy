using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSpawner : MonoBehaviour, Action.IMainActions
{
    Action action;
    Vector2 position= Vector2.zero;

    [SerializeField] Camera cam;
    Plane groundPlane = new Plane(Vector3.up, 0);
    bool isClicked = false;
    public void OnClick_1(InputAction.CallbackContext context)
    {
        isClicked = true;

        var query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(SettingComponentData));

        var setting = query.GetSingleton<SettingComponentData>();
        var ray = cam.ScreenPointToRay(position);
        if (groundPlane.Raycast(ray, out var enter))
        {
            var position = ray.GetPoint(enter);
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.Instantiate(setting.targetEntity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new LocalTransform { Position = position, Scale = 1 });
        }
    }

    public void OnTouchPosition(InputAction.CallbackContext context)
    {
        position = context.ReadValue<Vector2>();
    }

    void Awake()
    {
        action = new Action();
        action.main.SetCallbacks(this);
    }

    private void OnEnable()
    {
        action.main.Enable();
    }

    private void OnDisable()
    {
        action.main.Disable();
    }

    
    void Update()
    {
        if (isClicked)
        {
            isClicked = false;
           

        }
    }
}
