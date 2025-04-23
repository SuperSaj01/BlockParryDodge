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

    public Vector2 cameraInput;

    public bool isJumping;
    public bool isRolling;
    public bool isBlocking;

    public bool basicHit;

    public event EventHandler OnJumpBtnPressed;
    public event EventHandler OnRollBtnPressed;
    public event EventHandler OnAttackBtnPressed;
    public event EventHandler OnAbilityBtnPressed;
    public event EventHandler OnLockCameraPressed;
    public event EventHandler OnCriticalBtnPressed;
    public event EventHandler OnMenuTogglePressed;

    private void OnEnable() 
    {

        animManager = GetComponent<AnimatorManager>();   

        if(playerControls == null)
        {   

            ///Invokes all events respective to a button being pressed.
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            //isRunning changes depending if running button is held down
            playerControls.PlayerActions.Running.performed += i => isRunning = true;
            playerControls.PlayerActions.Running.canceled += i => isRunning = false;
            //isBlocking changes depending if running button is held down
            playerControls.PlayerActions.Blocking.performed += i => isBlocking = true;
            playerControls.PlayerActions.Blocking.canceled += i => isBlocking = false;

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
            playerControls.PlayerActions.Ability.performed += i => 
            {
                OnAbilityBtnPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.Critical.performed += i => 
            {
                OnCriticalBtnPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.PlayerActions.LockCamera.performed += i => 
            {
                OnLockCameraPressed?.Invoke(this, EventArgs.Empty);
            };
            playerControls.UserInput.ToggleMenu.performed += i => 
            {
                OnMenuTogglePressed?.Invoke(this, EventArgs.Empty);
            };
        }

        playerControls.Enable();
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

        animManager.UpdateAnimatorValues(0, moveAmount, isRunning, isBlocking); //Updates animator based on input
    }
    
    public bool GetRunningBool()
    {
        return isRunning;
    }

}
