using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimatorManager : MonoBehaviour
{

    PlayerManager playerManager;
    Animator anim;
    int horizontal;
    int vertical;

    private void Awake () 
    {
        //Assign references to variables by grabbing the componenets 
        anim = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");   
    }

    ///Summary of method
    /// This method is responsible for updating the animator values for the player character
    /// It takes in the horizontal and vertical movement values, and the isRunning and isBlocking bools
    /// It then snaps the values to the nearest whole number to ensure the animations are smooth
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isRunning, bool isBlocking)
    {

        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if(horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if(horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1f;
        }
        else if( horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if(horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1f;
        } 
        else
        {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if(verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if(verticalMovement > 0.55f)
        {
            snappedVertical = 1f;
        }
        else if(verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if(verticalMovement < -0.55f)
        {
            snappedVertical = -1f;
        } 
        else
        {
            snappedVertical = 0;
        }
        #endregion
        if(isRunning && (horizontalMovement != 0 ||  verticalMovement != 0))
        {
            snappedVertical = 2;
        }
        if(isBlocking)
        {
            if(horizontalMovement > 0)
            {
                snappedHorizontal = -1.5f;
            }
            else
            {
                snappedHorizontal = -2f;
            }

            if(verticalMovement > 0)
            {
                snappedVertical = 1f;
            }
            else
            {
                snappedVertical = 0f;
            }
        }

        anim.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        anim.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    ///Summary of method
    /// This method is responsible for playing the action animations for the player character
    public void PlayActionAnimation(string animationID, bool isInteracting, bool IsOwner)
    {
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(animationID, 0.2f);

        if(!IsOwner) return; // must ensure non-owners call the serverRPC on the local players game which is an error!
        playerManager.characterNetworkManager.NotifyServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, animationID, isInteracting);

    }


    public bool GetBool()
    {
        //Getter of isInteracting bool
        return anim.GetBool("isInteracting");
    }
}
