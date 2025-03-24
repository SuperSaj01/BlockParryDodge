using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [Header("Managers")]
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager camManager;

    public ulong clientId {get; private set;}

    //temp
    public WeaponSO tempWempSO;

    bool isRunning;
    bool isBlocking;

    public bool isInteracting;

    private void Awake() 
    {
        base.Awake();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        camManager = CameraManager.instance;
        DontDestroyOnLoad(gameObject);

        playerCombatManager.DequipWeapon();

        clientId = NetworkManager.Singleton.LocalClientId;
    }

    private void Start()
    {
        base.Start();
        
        OnCharacterChange();
    }

    protected override void Update()
    {
        base.Update();

        UpdatePlayers();
        if(!IsOwner) return;
        //Handle states
        //Need to move and change
       if(isBlocking) isRunning = false;
        isBlocking = inputManager.isBlocking;
        playerCombatManager.HandleBlocking(isBlocking);
       


        HandleCamera();
        HandleMovementLocomotion();
        ResetFlags();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCombatManager.EquipWeapon(1, IsOwner);
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
            CameraManager.instance.player = this;
        }

        WorldManager.instance.AddPlayer(this, NetworkManager.Singleton.LocalClientId);
    } 
    
    void OnCharacterChange()
    {
        playerCombatManager.InitiliaseStats(characterStatHandler.rollWindow, characterStatHandler.parryWindow);
    }

    #region enabling and disabling
    private void OnEnable()
    {
        //Input Events
        inputManager.OnAttackBtnPressed += _OnAttackBtnPressed; // attack needs to be changed to interact
        inputManager.OnJumpBtnPressed += _OnJumpBtnPressed; // jump
        inputManager.OnRollBtnPressed += _OnRollBtnPressed;
        inputManager.OnLockCameraPressed += _OnLockCameraPressed; // lock camera

        //World Events
        WorldManager.instance.OnLoadSceneEvent += _OnSceneLoaded;

    }

    private void OnDisable()
    {
        inputManager.OnAttackBtnPressed -= _OnAttackBtnPressed;
        inputManager.OnJumpBtnPressed -= _OnJumpBtnPressed;
        inputManager.OnRollBtnPressed -= _OnRollBtnPressed;
        inputManager.OnLockCameraPressed -= _OnLockCameraPressed;
        WorldManager.instance.OnLoadSceneEvent -= _OnSceneLoaded;
    }
    #endregion

    protected override void UpdatePlayers()
    {
        if(IsOwner)
        {
            //position
            characterNetworkManager.netPosition.Value = transform.position;
            //rotation
            characterNetworkManager.netRotation.Value = transform.rotation;
            //animation
            characterNetworkManager.netMoveAmount.Value = inputManager.moveAmount;
            //characterNetworkManager.netIsRunning.Value = inputManager.GetBlockingBool();
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
            //isBlocking = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning, isBlocking);
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
    

    #region Communicate with other scripts
        #region Event

    void _OnLockCameraPressed(object sender, EventArgs e)
    {
        playerLocomotion.fixedRotation = !playerLocomotion.fixedRotation;
    }

    private void _OnJumpBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting) return;
        playerLocomotion.HandleJumping();
    }
    private void _OnRollBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting) return;    
        playerLocomotion.HandleRolling();
        PlayActionAnimation("Rolling", true, IsOwner);        
    }

    private void _OnAttackBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting) return;
        playerCombatManager.AttackBtnPressed();
    } 
        #endregion

    public void HandleDamage(float damage)
    {
        if(playerCombatManager.ValidateDamage())
        {
            characterStatHandler.TakeDamage(damage);
        }
        else
        {
            Debug.Log("didnt do jack");
        }
    }    
    
    public void PlayActionAnimation(string animationID, bool isInteracting, bool IsOwner)
    {
        animatorManager.PlayActionAnimation(animationID, isInteracting, IsOwner);
    }
    public void EquipWeapon(int weaponID, bool IsOwner)
    {
        playerCombatManager.EquipWeapon(weaponID, IsOwner);
    }
    public void DequipWeapon()
    {
        playerCombatManager.DequipWeapon();
    }
    public void TakeDamage(float damage)
    {
        characterStatHandler.TakeDamage(damage);
    }

    public void SetNewHealthAmt(float newHealth, bool IsOwner)
    {
        characterStatHandler.NewHealthAmt(newHealth, IsOwner);
    }
    #endregion
    private void ResetFlags()
    {
        if(isInteracting)
        {
            inputManager.isJumping = false;
            inputManager.isRolling = false;
            inputManager.basicHit = false;
        }
    }

    private void _OnSceneLoaded()
    {
        WorldManager.instance.AddPlayer(this, NetworkManager.Singleton.LocalClientId);
    }
}