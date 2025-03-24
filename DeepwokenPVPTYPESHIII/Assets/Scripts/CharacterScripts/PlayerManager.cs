using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [Header("Managers")]
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
    PlayerLocomotion playerLocomotion;
     //to be changed to playerStatHandler possibly?
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CharacterStatHandler characterStatHandler; //to be changed to playerStatHandler possibly?
    AnimatorManager animatorManager;
    PlayerCombatManager playerCombatManager;
    public CharacterNetworkManager characterNetworkManager {get; private set;}
    CameraManager camManager;

    public ulong ClientID {get; private set;}

    //temp
    public WeaponSO tempWempSO;

    bool isRunning;

    public bool isInteracting;

    private void Awake() 
    {
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
        base.Awake();
        playerLocomotion = GetComponent<PlayerLocomotion>();
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        camManager = CameraManager.instance;
        DontDestroyOnLoad(gameObject);

        playerCombatManager.DequipWeapon();

        ClientID = NetworkManager.Singleton.LocalClientId;
    }

    private void Start()
    {
        IgnoreMyOwnColliders();    
    }

    void Update()
    {
<<<<<<< HEAD
<<<<<<< Updated upstream
        UpdatePlayers();
        if(!IsOwner) return;
=======
        base.Update();
        UpdatePlayers();
        if(!IsOwner) return;
        //Handle states
        //Need to move and change
       if(isBlocking) isRunning = false;

        isBlocking = inputManager.isBlocking;
        playerCombatManager.HandleBlocking(isBlocking);
       


>>>>>>> Stashed changes
=======
        UpdatePlayers();
        if(!IsOwner) return;
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        HandleCamera();
        HandleMovementLocomotion();
        ResetFlags();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerCombatManager.EquipWeapon(1, IsOwner);
        }
<<<<<<< HEAD
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
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
    

    private void OnEnable()
    {
        //Input Events
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        inputManager.OnAttackBtnPressed += _OnAttackBtnPressed;// attack needs to be changed to interact
        inputManager.OnJumpBtnPressed += _OnJumpBtnPressed;// jump
        inputManager.OnRollBtnPressed += _OnRollBtnPressed;
        inputManager.OnLockCameraPressed += _OnLockCameraPressed;// lock camera
<<<<<<< HEAD
=======
        inputManager.OnAttackBtnPressed += _OnAttackBtnPressed; // attack needs to be changed to interact
        inputManager.OnJumpBtnPressed += _OnJumpBtnPressed; // jump
        inputManager.OnRollBtnPressed += _OnRollBtnPressed;
        inputManager.OnLockCameraPressed += _OnLockCameraPressed; // lock camera
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)

        //World Events
        WorldManager.instance.OnLoadSceneEvent += _OnSceneLoaded;

    }

    private void OnDisable()
    {
        inputManager.OnAttackBtnPressed -= _OnAttackBtnPressed;
        inputManager.OnJumpBtnPressed -= _OnJumpBtnPressed;
        inputManager.OnRollBtnPressed -= _OnRollBtnPressed;
        inputManager.OnLockCameraPressed -= _OnLockCameraPressed;
<<<<<<< HEAD
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        WorldManager.instance.OnLoadSceneEvent -= _OnSceneLoaded;
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
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
            characterNetworkManager.netIsRunning.Value = inputManager.GetRunningBool();

            //Stats:
            //health
            characterNetworkManager.netCurrentHealth.Value = characterStatHandler.currentHealth;
            //posture
            characterNetworkManager.netCurrentPosture.Value = characterStatHandler.currentPosture;
            
<<<<<<< HEAD
=======
            //characterNetworkManager.netIsRunning.Value = inputManager.GetBlockingBool();
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
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
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
            isRunning = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning);

            //Stats:
            //health
            characterStatHandler.currentHealth = characterNetworkManager.netCurrentHealth.Value;
            //posture
            // characterStatHandler.currentPosture = characterNetworkManager.netCurrentPosture.Value;
            
        }
    }

    public void RequestDamage(targetId, ownId, damage);
    {

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
<<<<<<< HEAD
=======
            //isBlocking = characterNetworkManager.netIsRunning.Value;
            animatorManager.UpdateAnimatorValues(0, characterNetworkManager.netMoveAmount.Value, isRunning, isBlocking);
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
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
            inputManager.isRoling = false;
            inputManager.basicHit = false;
        }
    }

    private void _OnSceneLoaded()
    {
        WorldManager.instance.AddPlayer(this, NetworkManager.Singleton.LocalClientId);
    }
}
