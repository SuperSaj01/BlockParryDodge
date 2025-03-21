using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [Header("Managers")]
    PlayerStateManager playerStateManager;
    PlayerLocomotion playerLocomotion;
     //to be changed to playerStatHandler possibly?
    InputManager inputManager;
    
    CameraManager camManager;

    [SerializeField] public ulong clientID {get; private set;}

    //temp
    public WeaponSO tempWempSO;

    bool isRunning;
    bool isBlocking;
    public bool isInteracting;

    protected override void Awake() 
    {
        base.Awake();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerStateManager = GetComponent<PlayerStateManager>();
        inputManager = GetComponent<InputManager>();

        camManager = CameraManager.instance;

        DontDestroyOnLoad(gameObject);

        playerCombatManager.DequipWeapon();

        clientID = NetworkManager.Singleton.LocalClientId;
    }

    protected override void Start()
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
        if(isBlocking) isRunning = false;


        HandleCamera();
        HandleMovementLocomotion();
        ResetFlags();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCombatManager.EquipWeapon(1, IsOwner);
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log(this.OwnerClientId);
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

            PlayerDatabase.AddPlayer(clientID, this);
        }
    } 
    
    void OnCharacterChange()
    {
        playerCombatManager.InitiliaseStats(characterStatHandler.rollWindow, characterStatHandler.parryWindow);
    }

    #region Enable n Disable
    private void OnEnable()
    {
        //World Events
        WorldManager.instance.OnLoadSceneEvent += _OnSceneLoaded;

    }

    private void OnDisable()
    {
        WorldManager.instance.OnLoadSceneEvent -= _OnSceneLoaded;
    }
    #endregion


    protected override void UpdatePlayers()
    {
        base.UpdatePlayers();
        if(IsOwner)
        {
            characterNetworkManager.netMoveAmount.Value = inputManager.moveAmount;
            characterNetworkManager.netIsRunning.Value = playerStateManager.isRunning;
            //characterNetworkManager.netIsRunning.Value = inputManager.GetBlockingBool();
        }
        else
        {
            inputManager.moveAmount = characterNetworkManager.netMoveAmount.Value;
            playerStateManager.isRunning = characterNetworkManager.netIsRunning.Value;
            //isBlocking = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning, isBlocking);
        }
    }

    void HandleMovementLocomotion()
    {
            playerLocomotion.hInput = inputManager.movementInput.x;
            playerLocomotion.vInput = inputManager.movementInput.y;
            
            playerLocomotion.isRunning = inputManager.GetRunningBool();

            playerLocomotion.HandleAllMovement();
            playerLocomotion.SetSpeed(playerStateManager.GetCurrentState().GetMovementSpeed());
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
        playerCombatManager.HandleIFrames("Rolling");       
    }

    private void _OnAttackBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting) return;
        playerCombatManager.AttackBtnPressed();
    } 
        #endregion

    
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
    public void HandleDamage(float damage)
    {
        if(playerCombatManager.ValidateDamage())
        {
            characterStatHandler.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Didnt do jack");
        }
    }
    #endregion
    private void ResetFlags()
    {
        if(isInteracting)
        {
            inputManager.jumped = false;
            inputManager.rolled = false;
            inputManager.basicHit = false;
        }
    }

    private void _OnSceneLoaded()
    {

    }
}
