using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private float idlingSpeed = 0f;

    public override void EnterState(PlayerStateManager player)
    {
        base.EnterState(player);
    }
    public override void UpdateState(PlayerStateManager player)
    {
        
    }
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("exiting idle state");
    }
    
    
    public new float GetMovementSpeed => idlingSpeed;
}
