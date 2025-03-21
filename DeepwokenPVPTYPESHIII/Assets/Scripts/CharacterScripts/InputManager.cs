using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls; 
    AnimatorManager animManager;

    public Vector2 movementInput;
    public float moveAmount;
    public float horizontal;
    public float vertical;

    public float cameraInputX;
    public float cameraInputY;

    private bool isRunning;
    private bool isBlocking;

    public Vector2 cameraInput;

    public bool jumped;
    public bool rolled;

    public bool basicHit;

    public event EventHandler OnJumpBtnPressed;
    public event EventHandler OnRollBtnPressed;
    public event EventHandler OnAttackBtnPressed;
    public event EventHandler OnLockCameraPressed;
    
    public event EventHandler OnRunButtonPressed;
    public event EventHandler OnRunButtonReleased;

    public event EventHandler OnBlockButtonPressed;
    public event EventHandler OnBlockButtonReleased;

    private void Awake() 
    { 
        
    }

    private void OnEnable() 
    {

        animManager = GetComponent<AnimatorManager>();   

        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Running.performed += i => 
            {
                OnRunButtonPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.Running.canceled += i => 
            {
                OnRunButtonReleased?.Invoke(this, EventArgs.Empty);
            };

            playerControls.PlayerActions.Jumping.performed += i => 
            {
                OnJumpBtnPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.Rolling.performed += i => 
            {
                OnRollBtnPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.BasicAttack.performed += i => 
            {
                OnAttackBtnPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.LockCamera.performed += i => 
            {
                OnLockCameraPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.Blocking.performed += i => 
            {
                OnBlockButtonPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.Blocking.canceled += i => 
            {
                OnBlockButtonReleased?.Invoke(this, EventArgs.Empty);
            };
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        HandleAllInputs();    
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        animManager.UpdateAnimatorValues(0, moveAmount, isRunning, isBlocking);
    }
    
    public bool GetRunningBool()
    {
        return isRunning;
    }
    public bool GetBlockingBool()
    {
        return isBlocking;
    }

}
