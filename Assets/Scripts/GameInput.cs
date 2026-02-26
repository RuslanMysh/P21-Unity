using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnSprintStarted;
    public event EventHandler OnSprintCancelled;
    public event EventHandler OnPauseAction;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Больше одного GameInput!");
        }

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        inputActions.Player.Sprint.started += Sprint_started;
        inputActions.Player.Sprint.canceled += Sprint_canceled;
        inputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= Interact_performed;
        inputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        inputActions.Player.Sprint.started -= Sprint_started;
        inputActions.Player.Sprint.canceled -= Sprint_canceled;
        inputActions.Player.Pause.performed -= Pause_performed;

        inputActions.Dispose();
    }   

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintCancelled?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
