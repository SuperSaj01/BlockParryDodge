using System;
using System.Threading;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerState currentState;
    private PlayerIdleState idleState = new PlayerIdleState();
    private PlayerWalkState walkState = new PlayerWalkState();
    private PlayerRunState runState = new PlayerRunState();
    private PlayerJumpState jumpState = new PlayerJumpState();
    private PlayerBlockState blockState = new PlayerBlockState();

    [SerializeField] InputManager inputManager;
    AnimatorManager animatorManager;

    public bool isJumping = false;
    public bool isRunning = false;
    public bool isBlocking = false;
    public bool isAttacking = false;
    public bool isRolling = false;



    void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
    }
    void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    private void OnEnable()
    {
        //Input Events
        inputManager.OnAttackBtnPressed += _OnAttackBtnPressed; // attack needs to be changed to interact
        inputManager.OnJumpBtnPressed += _OnJumpBtnPressed; // jump
        inputManager.OnRollBtnPressed += _OnRollBtnPressed;
        inputManager.OnLockCameraPressed += _OnLockCameraPressed; // lock camera

        inputManager.OnRunButtonPressed += _OnRunButtonPressed;
        inputManager.OnRunButtonReleased += _OnRunButtonReleased;

        inputManager.OnBlockButtonPressed += _OnBlockButtonPressed;
        inputManager.OnBlockButtonPressed += _OnBlockButtonReleased;
    }
    private void OnDisable()
    {
        
        inputManager.OnAttackBtnPressed -= _OnAttackBtnPressed;
        inputManager.OnJumpBtnPressed -= _OnJumpBtnPressed;
        inputManager.OnRollBtnPressed -= _OnRollBtnPressed;
        inputManager.OnLockCameraPressed -= _OnLockCameraPressed;
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(PlayerState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    #region events
    void _OnAttackBtnPressed(object sender, EventArgs e)
    {
        isAttacking = true;
    }
    void _OnJumpBtnPressed(object sender, EventArgs e)
    {

    }
    void _OnRollBtnPressed(object sender, EventArgs e)
    {
        isRolling = true;
    }
    void _OnLockCameraPressed(object sender, EventArgs e)
    {
        
    }
    void _OnRunButtonPressed(object sender, EventArgs e)
    {
        isRunning = true;
    }
    void _OnRunButtonReleased(object sender, EventArgs e)
    {
        isRunning = false;
    }
    void _OnBlockButtonPressed(object sender, EventArgs e)
    {
        isBlocking = true;
    }
    void _OnBlockButtonReleased(object sender, EventArgs e)
    {
        isBlocking = false;
    }
    #endregion
}
