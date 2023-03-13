using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[UpdateAfter(typeof(InputSystem))]
[BurstCompile]
partial struct MovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");

        InputSystem.inputActionMap.FindAction("Click").performed += OnMouseClick;
        InputSystem.inputActionMap.FindAction("MouseMove").performed += OnMouseMove;
        InputSystem.inputActionMap.FindAction("Movement").performed += OnMovement;

        if (InputSystem.inputActionMap.enabled)
        {
            InputSystem.inputActionMap.Disable();
        }

        var newAction = InputSystem.inputActionMap.AddAction("Combo");
        newAction.AddCompositeBinding("OneModifier").
            With("Binding", "<Keyboard>/w").
            With("Binding", "<Keyboard>/a").
            With("Binding", "<Keyboard>/s").
            With("Binding", "<Keyboard>/d").
            With("Modifier", "<Keyboard>/leftCtrl");

        newAction.performed += OnCombo;

        if (!InputSystem.inputActionMap.enabled)
        {
            InputSystem.inputActionMap.Enable();
        }
    }

    public void OnDestroy(ref SystemState state)
    {
        Debug.Log("MovementSystem OnDestroy!");
    }

    public void OnUpdate(ref SystemState state)
    {

    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked!");
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        //Debug.Log("Mouse Move!" + context.ReadValue<Vector2>());
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        var movementAction = InputSystem.inputActionMap["Movement"];

        if (InputSystem.IsKeyDown(movementAction, "W"))
        {
            Debug.Log("MovementSystem w");
        }
        if (InputSystem.IsKeyDown(movementAction, "A"))
        {
            Debug.Log("MovementSystem a");
        }
        if (InputSystem.IsKeyDown(movementAction, "S"))
        {
            Debug.Log("MovementSystem s");
        }
        if (InputSystem.IsKeyDown(movementAction, "D"))
        {
            Debug.Log("MovementSystem d");
        }
    }

    public void OnCombo(InputAction.CallbackContext context)
    {
        var comboAction = InputSystem.inputActionMap["Combo"];

        if (InputSystem.IsKeyDown(comboAction, "W"))
        {
            Debug.Log("Combo w");
        }
        if (InputSystem.IsKeyDown(comboAction, "A"))
        {
            Debug.Log("Combo a");
        }
        if (InputSystem.IsKeyDown(comboAction, "S"))
        {
            Debug.Log("Combo s");
        }
        if (InputSystem.IsKeyDown(comboAction, "D"))
        {
            Debug.Log("Combo d");
        }
    }
}
