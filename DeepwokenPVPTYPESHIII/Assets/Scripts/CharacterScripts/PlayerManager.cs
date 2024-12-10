using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [Header("Managers")]
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CharacterStatHandler characterStatHandler; //to be changed to playerStatHandler possibly?
    AnimatorManager animatorManager;
    public CharacterNetworkManager characterNetworkManager {get; private set;}
    CameraManager camManager;

    bool isRunning;

    public bool isInteracting;

    private void Awake() 
    {
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
        camManager = CameraManager.instance;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        IgnoreMyOwnColliders();
    }

    void Update()
    {
        UpdatePlayers();
        if(!IsOwner) return;
        HandleCamera();
        HandleMovementLocomotion();
        ResetFlags();
        if(isInteracting) return;
        HandleJumping();
        HandleAttacking();
        HandleRolling();

        if(Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(2);
        }

    }
    
    private void FixedUpdate()
    {
        if(!IsOwner) return;
    }

    private void LateUpdate()
    {
        if(!IsOwner) return;
        CameraManager.instance.HandleAllCameraMovement();
        isInteracting = animatorManager.GetBool();
    }

    public override void OnNetworkSpawn()
    {
            base.OnNetworkSpawn();

            if(IsOwner)
            {
                Debug.Log(CameraManager.instance.name);
                CameraManager.instance.player = this;
            }
    } 
    

    void UpdatePlayers()
    {
        if(IsOwner)
        {
            //position
            characterNetworkManager.netPosition.Value = transform.position;
            //rotation
            characterNetworkManager.netRotation.Value = transform.rotation;
            //animation
            characterNetworkManager.netMoveAmount.Value = inputManager.moveAmount;
            characterNetworkManager.netIsRunning.Value = inputManager.GetRunningBool();

            //Stats:
            //health
            characterNetworkManager.netCurrentHealth.Value = characterStatHandler.currentHealth;
            //posture
            characterNetworkManager.netCurrentPosture.Value = characterStatHandler.currentPosture;
            
        }
        else
        {
            //movement
            transform.position = Vector3.SmoothDamp(transform.position,
            characterNetworkManager.netPosition.Value,
            ref characterNetworkManager.netPositionVel,
            characterNetworkManager.netPositionSmoothTime);
            
            //rotation
            transform.rotation = Quaternion.Slerp(transform.rotation,
             characterNetworkManager.netRotation.Value,
             characterNetworkManager.rotationSpeed);

            //animation
            inputManager.moveAmount = characterNetworkManager.netMoveAmount.Value;
            isRunning = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning);

            //Stats:
            //health
            characterStatHandler.currentHealth = characterNetworkManager.netCurrentHealth.Value;
            //posture
            characterStatHandler.currentPosture = characterNetworkManager.netCurrentPosture.Value;
            
        }
    }

    void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoredColliders = new List<Collider>();

        foreach(Collider col in damageableColliders)
        {
            ignoredColliders.Add(col);
        }
        ignoredColliders.Add(characterControllerCollider);

        foreach(Collider col in ignoredColliders)
        {
            foreach(Collider otherCol in ignoredColliders)
            {
                Physics.IgnoreCollision(col, otherCol, true);
            }
        }
    }

    void HandleMovementLocomotion()
    {
            playerLocomotion.hInput = inputManager.movementInput.x;
            playerLocomotion.vInput = inputManager.movementInput.y;
            
            playerLocomotion.isRunning = inputManager.GetRunningBool();

            inputManager.HandleAllInputs();
            playerLocomotion.HandleAllMovement();
    }

    void HandleCamera()
    {
            camManager.camHInput = inputManager.cameraInput.x;
            camManager.camVInput = inputManager.cameraInput.y;
    }    

    private void HandleJumping()
    {
        if(inputManager.jumped)
        {
            playerLocomotion.HandleJumping();
            inputManager.jumped = false;

        }
    }
    private void HandleRolling()
    {
        if(inputManager.rolled)
        {
            inputManager.rolled = false;
            playerLocomotion.HandleRolling();
            PlayActionAnimation("Rolling", true, IsOwner);
        }
    }

    private void HandleAttacking()
    {
        if(inputManager.basicHit)
        {
            inputManager.basicHit = false;
            Debug.Log("Hello");
            PlayActionAnimation("Basic Hit", true, IsOwner);
        }
    } 

    public void PlayActionAnimation(string animationID, bool isInteracting, bool IsOwner)
    {
        animatorManager.PlayActionAnimation(animationID, isInteracting, IsOwner);
    }

    public void TakeDamage(int damage)
    {
        characterStatHandler.TakeDamage(damage);
    }

    private void ResetFlags()
    {
        if(isInteracting)
        {
            inputManager.jumped = false;
            inputManager.rolled = false;
            inputManager.basicHit = false;
        }
    }
}
