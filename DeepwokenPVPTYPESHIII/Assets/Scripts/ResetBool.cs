using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool = "isInteracting";

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //when an animation state of an action has stopped then the interacting flag is set to false
        animator.SetBool(isInteractingBool, false); 
    }
}
