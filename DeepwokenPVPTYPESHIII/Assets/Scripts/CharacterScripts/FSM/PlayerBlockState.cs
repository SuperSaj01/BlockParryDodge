using UnityEngine;

public class PlayerBlockState : PlayerState
{
    public override void EnterState(PlayerStateManager player)
    {

    }
    public override void UpdateState(PlayerStateManager player)
    {

    }
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("exiting block state");
    }

    public new float GetMovementSpeed => 1f;
}
