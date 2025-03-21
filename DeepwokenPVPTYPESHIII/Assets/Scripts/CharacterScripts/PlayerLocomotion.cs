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
    public bool isRunning;

    [Header("Jumping")]
    private bool isJumping; 
    private float jumpHeight = 0.15f;
    
    [Header("Gravity")]
    private bool isGrounded;
    private float gravityIntensity = -90f;        
    public LayerMask groundLayer;

    [Header("Camera")]
    Transform cam;
    private float rotationSpeed = 15f;

    [Header("Rotation")]
    Quaternion playerRotation = Quaternion.identity;
    public bool fixedRotation = false;


    [Header("HandlingFalling")]
    private float maxDistance = 0.16f;
    private float inAirTimer;
    private float inAirSpeed = 2.5f;
    private float rayCastheighOffset = 0.25f;


    public float dashTime;
    public float dashSpeed;

    private void Awake()
    {
        isGrounded = true;//!!
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
        Vector3 forward = CameraManager.instance.transform.forward;
        forward.y = 0;
        moveDirection = forward * vInput + CameraManager.instance.transform.right * hInput;
        moveDirection.Normalize();

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

        targetDirection = CameraManager.instance.transform.forward * vInput;
        targetDirection += CameraManager.instance.transform.right * hInput;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if(!fixedRotation)
        {
            if(targetDirection == Vector3.zero) targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 cameraForward = CameraManager.instance.transform.forward;

            cameraForward.y = 0f;
            cameraForward.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;

        if(Physics.SphereCast(rayCastOrigin + new Vector3(0, rayCastheighOffset, 0), 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
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
            velocity.y += gravityIntensity * inAirTimer * Time.deltaTime;
        }
        else
        {
            if(velocity.y < 0)
            {
                velocity.y = -1f;
            }
        }

        playerController.Move(velocity * Time.deltaTime);
        
    }// fix checking isGrounded


    //To be worked on the physics
    public void HandleJumping() 
    {
        if(!isGrounded) return;
        isJumping = true;
        float jumpPos = Mathf.Sqrt(jumpHeight * -2f * gravityIntensity);
        velocity.y = jumpPos;
    }


    public void HandleRolling()
    {   
        StartCoroutine(Roll());
    }

    IEnumerator Roll()
    {
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            playerController.Move(lastfacingDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }
    
    public void SetSpeed(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }

    
}
