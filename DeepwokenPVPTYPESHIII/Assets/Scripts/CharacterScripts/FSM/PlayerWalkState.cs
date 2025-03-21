using UnityEngine;

public class PlayerWalkState : PlayerState
{

    private float walkingSpeed = 3f; 

    public override void EnterState(PlayerStateManager player)
    {

    }
    public override void UpdateState(PlayerStateManager player)
    {

    }
    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("exiting walking state");
    }

    public new float GetMovementSpeed => walkingSpeed;
}
