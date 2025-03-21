using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public override void EnterState(PlayerStateManager player)
    {

    }
    public override void UpdateState(PlayerStateManager player)
    {

    }
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("exiting jump state");
    }

    public new float GetMovementSpeed => 1f;
}
