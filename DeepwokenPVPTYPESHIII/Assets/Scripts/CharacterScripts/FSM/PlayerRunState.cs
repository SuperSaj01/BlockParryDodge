using UnityEngine;

public class PlayerRunState : PlayerState
{

    private float runningSpeed = 7.5f;

    public override void EnterState(PlayerStateManager player)
    {

    }
    public override void UpdateState(PlayerStateManager player)
    {

    }
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("exiting run state");
    }

    public new float GetMovementSpeed => runningSpeed;
}
