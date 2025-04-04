using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [Header("Managers")]
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager camManager;
    [SerializeField] GameMenu gameMenu;

    public ulong clientId {get; private set;}
    [SerializeField] Transform headPosition;
    private GameObject headObject;

    bool isRunning;
    public bool isBlocking {get; private set;}

    public bool isInteracting;
    bool isInMenu;

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

    protected override void Start()
    {
        base.Start();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Update()
    {
        base.Update();
        isInMenu = GameUIManager.instance.gameMenu.isInMenu;

        if(isInMenu) return;
        UpdatePlayers();
        if(!IsOwner) return;
        //Handle states
        //Need to move and change

        if(isBlocking) isRunning = false;
        isBlocking = inputManager.isBlocking;
        playerCombatManager.HandleBlocking(isBlocking);

        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(5);
        }
       


        HandleCamera();
        HandleMovementLocomotion();
        ResetFlags();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCombatManager.EquipWeapon(1, IsOwner);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerCombatManager.EquipWeapon(2, IsOwner);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            CharacterSO characterType = CharacterDatabase.GetCharacterTypeByID(1);
            OnCharacterChange(characterType);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {   
            
            CharacterSO characterType = CharacterDatabase.GetCharacterTypeByID(2);
            OnCharacterChange(characterType);
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
            PlayerDatabase.AddPlayer(clientId, this);
            CameraManager.instance.player = this;
        }
    } 
    
    void OnCharacterChange(CharacterSO characterType)
    {   
        if(headObject != null) Destroy(headObject);
        headObject = Instantiate(characterType.characterHead, headPosition.position, Quaternion.identity);
        headObject.transform.SetParent(headPosition);
        characterStatHandler.AssignStats(characterType);
        playerCombatManager.InitiliaseStats(characterStatHandler.rollWindow, characterStatHandler.parryWindow);
    }

    #region enabling and disabling
    private void OnEnable()
    {
        //Input Events
        inputManager.OnAttackBtnPressed += _OnAttackBtnPressed;
        inputManager.OnJumpBtnPressed += _OnJumpBtnPressed; 
        inputManager.OnRollBtnPressed += _OnRollBtnPressed;
        inputManager.OnLockCameraPressed += _OnLockCameraPressed;
        //UI events
        inputManager.OnMenuTogglePressed += _OnMenuTogglePressed;

        //World Events
        WorldManager.instance.OnLoadSceneEvent += _OnSceneLoaded;

    }

    private void OnDisable()
    {
        inputManager.OnAttackBtnPressed -= _OnAttackBtnPressed;
        inputManager.OnJumpBtnPressed -= _OnJumpBtnPressed;
        inputManager.OnRollBtnPressed -= _OnRollBtnPressed;
        inputManager.OnLockCameraPressed -= _OnLockCameraPressed;
        inputManager.OnMenuTogglePressed -= _OnMenuTogglePressed;
        WorldManager.instance.OnLoadSceneEvent -= _OnSceneLoaded;
    }
    #endregion

    protected override void UpdatePlayers()
    {
        base.UpdatePlayers();
        
        if(IsOwner)
        {
            //position
            //characterNetworkManager.netPosition.Value = transform.position;
            //rotation
            //characterNetworkManager.netRotation.Value = transform.rotation;
            //animation
            characterNetworkManager.netMoveAmount.Value = inputManager.moveAmount;
            //characterNetworkManager.netIsRunning.Value = inputManager.GetBlockingBool();
        }
        else
        {
            //movement
           // transform.position = Vector3.SmoothDamp(transform.position,
            //characterNetworkManager.netPosition.Value,
            //ref characterNetworkManager.netPositionVel,
            // characterNetworkManager.netPositionSmoothTime);
            
            //rotation
            //transform.rotation = Quaternion.Slerp(transform.rotation,
              //  characterNetworkManager.netRotation.Value,
                //characterNetworkManager.rotationSpeed);

            //animation
            inputManager.moveAmount = characterNetworkManager.netMoveAmount.Value;
            //isBlocking = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning, isBlocking);
        }
    }

    void HandleMovementLocomotion()
    {       
        //Assigns the input values to the player locomotion
        playerLocomotion.hInput = inputManager.movementInput.x;
        playerLocomotion.vInput = inputManager.movementInput.y;            
        
        //Assigns the running bool to the player locomotion based on the input 
        playerLocomotion.isRunning = inputManager.GetRunningBool();

        //Controls the methods that need to be called each frame on other scripts
        inputManager.HandleAllInputs();
        playerLocomotion.HandleAllMovement();
    }

 

    void HandleCamera()
    {
        //Assigns the input to the camera 
        camManager.camHInput = inputManager.cameraInput.x;
        camManager.camVInput = inputManager.cameraInput.y;
    }    
    

    #region Communicate with other scripts
        #region Event

    /// Summary of the method
    /// Event for when the lock camera button is pressed
    void _OnLockCameraPressed(object sender, EventArgs e)
    {
        playerLocomotion.fixedRotation = !playerLocomotion.fixedRotation;
    }

    /// Summary of the method
    /// Notifies other scripts that the jump button has been pressed
    private void _OnJumpBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting || isInMenu) return;
        playerLocomotion.HandleJumping();
    }

    /// Summary of the method
    /// Notifies other scripts that the roll button has been pressed
    private void _OnRollBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting || isInMenu) return;    
        playerLocomotion.HandleRolling();
        PlayActionAnimation("Rolling", true, IsOwner);        
    }

    /// Summary of the method
    /// Notifies other scripts that the attack button has been pressed
    private void _OnAttackBtnPressed(object sender, EventArgs e)
    {
        if(isInteracting || isInMenu) return;
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
    public void TakeDamage(float damage)
    {
        characterStatHandler.TakeDamage(damage);
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
        //WorldManager.instance.AddPlayer(this, NetworkManager.Singleton.LocalClientId);
    }

    private void _OnMenuTogglePressed(object sender, EventArgs e)
    {
        if(IsOwner)
        {
            GameUIManager.instance.gameMenu.ToggleLocalMenu();
        }
    }

    public void Died()
    {

    }
}