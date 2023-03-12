using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst;
using System;

[BurstCompile]
partial struct InputSystem : ISystem
{
    [SerializeField] private float mouseSensitivity;

    public static InputActionMap inputActionMap;

    public void OnCreate(ref SystemState state)
    {
        inputActionMap = new InputActionMap();
        AddMovementAction();
        inputActionMap.AddAction("MouseMove", binding: "<Mouse>/position").AddBinding("<Touch>/position");
        inputActionMap.AddAction("Click", binding: "<Touch>/press").AddBinding("<Mouse>/leftButton");
        inputActionMap.AddAction("Jump", binding: "<Keyboard>/space");

        inputActionMap.Enable();

        Debug.Log("InputSystem OnCreate!");
    }

    public void OnDestroy(ref SystemState state)
    {
        inputActionMap.Disable();

        Debug.Log("InputSystem OnDestroy!");
    }

    public void OnUpdate(ref SystemState state)
    {
    }

    private void AddMovementAction()
    {
        inputActionMap.AddAction("Movement", InputActionType.Value);

        var movementAction = inputActionMap["Movement"];

        movementAction.AddBinding("<Keyboard>/w");
        movementAction.AddBinding("<Keyboard>/a");
        movementAction.AddBinding("<Keyboard>/s");
        movementAction.AddBinding("<Keyboard>/d");
        movementAction.AddBinding("<Keyboard>/upArrow");
        movementAction.AddBinding("<Keyboard>/downArrow");
        movementAction.AddBinding("<Keyboard>/leftArrow");
        movementAction.AddBinding("<Keyboard>/rightArrow");
    }

    public static bool IsKeyDown(InputAction action, string key)
    {
        return action.activeControl.displayName.Equals(key, StringComparison.OrdinalIgnoreCase);
    }
}
