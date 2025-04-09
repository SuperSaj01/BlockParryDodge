using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    CharacterController playerController;

    [Header("InputManager")]
    public float hInput;
    public float vInput;

    [Header("Movement")]
    Vector3 moveDirection;
    Vector3 lastfacingDirection;
    Vector3 velocity;
    float movementSpeed;
    private float walkingSpeed = 3f; 
    private float runningSpeed = 7.5f;
    public bool isRunning;

    [Header("Jumping")]
    private bool isJumping;
    private float jumpHeight = 0.15f;
    
    [Header("Gravity")]
    private bool isGrounded;
    private const float GRAVITYINTENSITY = -90f;        
    public LayerMask groundLayer;

    [Header("Camera")]
    Transform cam;
    private const float ROTATIONSPEED = 15f;

    [Header("Rotation")]
    Quaternion playerRotation = Quaternion.identity;
    public bool fixedRotation = false;


    [Header("HandlingFalling")]
    private float maxDistance = 0.16f;
    private float inAirTimer;
    private float inAirSpeed = 2.5f;
    private float rayCastheighOffset = 0.25f;

    [Header("Rolling")]
    public float dashTime;
    public float dashSpeed;

    private void Awake()
    {
        movementSpeed = walkingSpeed;
        isGrounded = true;
        playerManager = GetComponent<PlayerManager>();
        playerController = GetComponent<CharacterController>();
    }

    public void HandleAllMovement()
    {			
        HandleFallingAndLanding();
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {  
        movementSpeed = HandleSpeed();
        Vector3 forward = CameraManager.instance.transform.forward; //finds the vector of the camera's forward direction
        forward.y = 0; //sets the y value to 0 so that the player does not move up or down
        moveDirection = forward * vInput + CameraManager.instance.transform.right * hInput;
        moveDirection.Normalize(); //normalizes the vector so that the player does not move faster when moving diagonally

        if(moveDirection != Vector3.zero)
        {
            lastfacingDirection = moveDirection;
            }

        moveDirection *= movementSpeed * Time.deltaTime;
        playerController.Move(moveDirection);   
    }
    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = CameraManager.instance.transform.forward * vInput; //If the player is moving in this direction then the target direction adds the camera's forward direction
        targetDirection += CameraManager.instance.transform.right * hInput; //If the player is moving in this direction then the target direction adds the camera's right direction
        targetDirection.y = 0;
        targetDirection.Normalize(); 

        if(!fixedRotation) //If the player is not fixed in rotation then the player will rotate towards the target direction
        {
            if(targetDirection == Vector3.zero) targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATIONSPEED * Time.deltaTime);
        }
        else
        {
            Vector3 cameraForward = CameraManager.instance.transform.forward;

            cameraForward.y = 0f;
            cameraForward.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATIONSPEED * Time.deltaTime); //Smooths the rotation of the player
        }
        
        transform.rotation = playerRotation; //assigns rotation
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;

        if(Physics.SphereCast(rayCastOrigin + new Vector3(0, rayCastheighOffset, 0), 0.2f, Vector3.down, out hit, maxDistance, groundLayer)) //Shoots a raycast and returns true if it hits an object with ground layer
        {
            inAirTimer = 0;
            isGrounded = true;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }

        if(!isGrounded)
        {
            inAirTimer += Time.deltaTime;
            velocity.y += GRAVITYINTENSITY * inAirTimer * Time.deltaTime; //Increases the velocity of the player as they fall
        }
        else
        {
            if(velocity.y < 0)
            {
                velocity.y = -1f;
            }
        }

        playerController.Move(velocity * Time.deltaTime);
    }


    //To be worked on the physics
    public void HandleJumping() 
    {
        if(!isGrounded) return; //dont jump if in air
        isJumping = true;
        float jumpPos = Mathf.Sqrt(jumpHeight * -2f * GRAVITYINTENSITY);
        velocity.y = jumpPos;
    }


    public void HandleRolling()
    {   
        StartCoroutine(Roll());
    }

    IEnumerator Roll()
    {
        float startTime = Time.time;

        while(Time.time < startTime + dashTime) //continues if current time is less than the time it started plus the dash time
        {
            playerController.Move(lastfacingDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }
        playerManager.canAttack = true;
    }
    
    ///Summary of the method
    ///This method is used to handle the speed of the player based on the state of the player and returns accordingly
    private float HandleSpeed()
    {
        if(isGrounded && !isRunning)
        {
            return walkingSpeed;
        }
        else if(isGrounded && isRunning)
        {
            return runningSpeed;
        }
        else if(!isGrounded)
        {
            return inAirSpeed;
        }
        else
        {
            return walkingSpeed;
        }
    }
    
}
