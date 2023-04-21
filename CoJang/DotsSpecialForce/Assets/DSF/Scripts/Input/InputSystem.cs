using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst;
using System;

[BurstCompile]
public partial class InputSystem : SystemBase
{
	[SerializeField] private float mouseSensitivity;

	public static InputActionMap inputActionMap;

	/// <summary> 자주 호출이 필요하면 MouseMove 액션에서 가져다 쓸 것. </summary>

	public static Vector2 MousePosition => inputActionMap["MouseMove"] == null ? Vector2.zero : inputActionMap["MouseMove"].ReadValue<Vector2>();
	public static InputAction MouseMoveAction => inputActionMap["MouseMove"];
	public static InputAction ClickAction => inputActionMap["Click"];
	public static InputAction JumpAction => inputActionMap["Jump"];
	public static InputAction MovementAction => inputActionMap["Movement"];

	[BurstCompile]
	protected override void OnCreate()
	{
		Debug.Log("InputSystem OnCreate!");

		inputActionMap = new InputActionMap();

		AddMovementAction();
		inputActionMap.AddAction("MouseMove", binding: "<Touch>/position").AddBinding("<Mouse>/position");
		inputActionMap.AddAction("Click", binding: "<Touch>/press").AddBinding("<Mouse>/leftButton");
		inputActionMap.AddAction("Jump", binding: "<Keyboard>/space");

		inputActionMap.Enable();
	}

	[BurstCompile]
	protected override void OnDestroy()
	{
		foreach (var action in inputActionMap.actions)
		{
			action.Dispose();
		}

		inputActionMap.Disable();
		inputActionMap.Dispose();

		Debug.Log("InputSystem OnDestroy!");
	}

	[BurstCompile]
	protected override void OnUpdate()
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
