using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : MonoBehaviour
{
    /*
    What I want this script to handle:
        - Spawning the player class (chosen)
        - Spawning the weapon (chosen)
            -can equip weapons. 
            -dynamically updates the weapon.
        - Handle what animations to be sent to animatorManager
        - Handle comboes
        - Handle Blocking, Parrying, Attacking
        - Hitting enemy doesnt deal damage more than once []
        - Sword touching enemy despite not swinging doesnt do damage []
    */

    private PlayerManager playerManager;

    [SerializeField] private GameObject placeWeaponLH;
    private WeaponSO currentWeaponSO;
    private GameObject currentWeaponObject;
<<<<<<< Updated upstream
=======

    [Header("Windows")]
    private float rollWindow;
    private float parryWindow;

    private bool canParry = true;
    bool isInIFrames = false;
    bool wasBlocking = false;
>>>>>>> Stashed changes
    //private GameObject currentWeapon


    [Header("Checking Hit")]
    private DealDamage dealDamage; // accesses the damage script on the weapon


    private int i = 0;


    [Header("Weapons Stats")]
    private float damage;

    void Awake()
    {
        
        playerManager = GetComponent<PlayerManager>();
        // make current wep spawn from SO
    }

<<<<<<< Updated upstream
=======
    public void HandleBlocking(bool isBlocking)
    {

        if(isBlocking && !wasBlocking && canParry)
        {
            HandleIFrames("Blocking");
        }

        wasBlocking = isBlocking;
    }

>>>>>>> Stashed changes
    public void AttackBtnPressed()
    {
        if(!HasWeaponEquipped() && dealDamage.isActive) return;
        dealDamage.isActive = true;
        if(i == 2)
        {
            i = 0;
        }
        
        playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[i++], true, playerManager.IsOwner);// needs to be in custom logic
        dealDamage.DetectCollision();

        StartCoroutine(SwingCooldown());
    }

<<<<<<< Updated upstream

    public void HandleIFrames()
    {
        //Check if player executed perfect dodge [OPTIONAL]
        //Disable hitbox for a few seconds. Perhaps use a coroutine
    }
=======
    #region IFrames
    public void HandleIFrames(string action)
    {
        //Check if player executed perfect dodge [OPTIONAL]
        if(action == "Rolling")
        {
            StartCoroutine(RollWindow());
        }
        if(action == "Blocking")
        {
            canParry = false;
            StartCoroutine(ParryWindow());

        }
        //Disable hitbox for a few seconds. Perhaps use a coroutine
    }
    private IEnumerator RollWindow()
    {
        isInIFrames = true;
        Debug.Log("is rolling iframes active inshaAllah");
        Debug.Log(rollWindow);
        yield return new WaitForSeconds(rollWindow);
        isInIFrames = false;
        Debug.Log("stopped iframes");
    }

    private IEnumerator ParryWindow()
    {
        isInIFrames = true;
        Debug.Log("is Parrying frames mashaAllah");
        yield return new WaitForSeconds(parryWindow);
        isInIFrames = false;
        canParry = true;
        Debug.Log("Stopped iframes");
    }

    public void InitiliaseStats(float rollWindow, float parryWindow)
    {
        this.rollWindow = rollWindow;
        Debug.Log(this.rollWindow);
        Debug.Log(rollWindow);
        this.parryWindow = parryWindow;
    }
    #endregion
>>>>>>> Stashed changes

    private IEnumerator SwingCooldown()
    {
        yield return new WaitForSeconds(currentWeaponSO.b_SwingSpeed);
        dealDamage.isActive = false;
    }

<<<<<<< Updated upstream
=======
    #region Dealing damage and recieving damage

    public void DealDamageToTarget(PlayerManager target)
    {
        //handle whatever client sided logic here
        
        //tell server to handle the damage
        ulong targetId = target.OwnerClientId;
        playerManager.characterNetworkManager.RequestDamageServerRpc(targetId, damage);
    }

    public bool ValidateDamage()
    {
        Debug.Log(isInIFrames);
        if(isInIFrames)
        {
            Debug.Log("No damage");
        }
        return !isInIFrames;
    }    

    #endregion

>>>>>>> Stashed changes
    #region Equipping/Dequipping Weapons
    private bool HasWeaponEquipped()
    {
        if(currentWeaponSO != null) return true; //Possibly add right hand in the mix later. Still need to handle player types firtst and weps  
        else return false;
    }

    public void EquipWeapon(int newWeaponID, bool IsOwner)
    {
        DequipWeapon();
        currentWeaponSO = ItemDatabse.GetWeaponByID(newWeaponID);
        Func<GameObject, GameObject, Vector3, Quaternion, GameObject> EquippingWeapon = (wepPrefab, parent, offset, rotation) => {
            GameObject temp = Instantiate(wepPrefab, parent.transform.position, rotation); //temp is the local reference to the weapon that will be spawned. 
            temp.transform.SetParent(parent.transform, true);
            temp.transform.localPosition += offset;
            dealDamage = temp.GetComponent<DealDamage>();
            playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[0], true, playerManager.IsOwner); //need to make by default the first animation in the array the pull out animation  
            return temp;    
        };
        
        currentWeaponObject = EquippingWeapon(currentWeaponSO.itemPrefab, placeWeaponLH, currentWeaponSO.offset, placeWeaponLH.transform.rotation);
        ChangeWeaponStats(currentWeaponSO);
        if(!IsOwner) return;
        playerManager.characterNetworkManager.NotifyServerOfInstantiatedObjectServerRpc(NetworkManager.Singleton.LocalClientId, newWeaponID);

    }
    public void DequipWeapon()
    {
        if(currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
        }
        currentWeaponSO = null;
    }
    #endregion

    private void ChangeWeaponStats(WeaponSO currentWeaponSO)
    {
        dealDamage.SetWeaponStats(this.GetComponent<PlayerManager>(),this, currentWeaponSO.b_Damage, currentWeaponSO.range, currentWeaponSO.boxColliderSize, currentWeaponSO.b_LayerMask);
    }

    

    

}
