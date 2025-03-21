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
        anim = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");   
    }

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
        if(isBlocking)
        {   
            if(horizontalMovement > 0) 
            {
                snappedHorizontal = 0.5f;
                if(verticalMovement > 0)
                {
                    snappedVertical = 0.5f;
                }
                else
                {
                    snappedVertical = 1;
                }
            }
            else
            {
                snappedHorizontal = 1;
            }
        }
        if(isRunning && (horizontalMovement != 0 ||  verticalMovement != 0))
        {
            snappedVertical = 2;
        }
        

        anim.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        anim.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayActionAnimation(string animationID, bool isInteracting, bool IsOwner)
    {
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(animationID, 0.2f);

        if(!IsOwner) return; // must ensure non-owners call the serverRPC on the local players game which is an error!
        playerManager.characterNetworkManager.NotifyServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, animationID, isInteracting);

    }

    public bool GetBool()
    {
        return anim.GetBool("isInteracting");
    }
}
