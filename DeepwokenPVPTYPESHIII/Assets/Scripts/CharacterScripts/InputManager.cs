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

    public bool jumped;
    public bool rolled;

    public bool basicHit;

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

            playerControls.PlayerActions.Running.performed += i => isRunning = true;
            playerControls.PlayerActions.Running.canceled += i => isRunning = false;

            playerControls.PlayerActions.Jumping.performed += i => jumped = true;
            playerControls.PlayerActions.Rolling.performed += i => rolled = true;

            playerControls.PlayerActions.BasicAttack.performed += i => basicHit = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        
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

        animManager.UpdateAnimatorValues(0, moveAmount, isRunning);
    }
    
    public bool GetRunningBool()
    {
        return isRunning;
    }

}
