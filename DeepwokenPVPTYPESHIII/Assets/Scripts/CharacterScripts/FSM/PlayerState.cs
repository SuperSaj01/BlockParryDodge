using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateManager player;

    public float horizontalMovment;
    public float verticalMovement;
    public float MoveAmount;

    public virtual void EnterState(PlayerStateManager player)
    {
        this.player = player;
    }
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void ExitState(PlayerStateManager player);

    public virtual float GetMovementSpeed() => 2f;

    protected void CheckForAttack()
    {
        if(player.isAttacking)
        {
           // player.SwitchState(player.PlayerAttackState);
        }
    }
}
