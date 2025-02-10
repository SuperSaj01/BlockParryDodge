using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WeaponSO currentWeaponSO;
    private GameObject currentWeaponObject;
    //private GameObject currentWeapon;

    private DealDamage dealDamage;

    private int i = 0;


    [Header("Weapons Stats")]
    private float damage;

    void Awake()
    {
        
        playerManager = GetComponent<PlayerManager>();
        // make current wep spawn from SO
    }

    public void AttackBtnPressed()
    {
        if(!HasWeaponEquipped()) return;
        if(i == 2)
        {
            i = 0;
        }
        
        playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[i++], true, playerManager.IsOwner);// needs to be in custom logic
    
        
        
    }


    public void HandleIFrames()
    {
        //Check if player executed perfect dodge [OPTIONAL]
        //Disable hitbox for a few seconds. Perhaps use a coroutine
    }

    #region Equipping/Dequipping Weapons
    private bool HasWeaponEquipped()
    {
        if(currentWeaponSO != null) return true; //Possibly add right hand in the mix later. Still need to handle player types firtst and weps  
        else return false;
    }

    public void EquipWeapon(WeaponSO newWeapon)
    {
        DequipWeapon();
        currentWeaponSO = newWeapon;
        Func<GameObject, GameObject, Vector3, Quaternion, GameObject> EquippingWeapon = (wepPrefab, parent, offset, rotation) => {
            GameObject temp = Instantiate(wepPrefab, parent.transform.position, rotation); //temp is the local reference to the weapon that will be spawned. 
            temp.transform.SetParent(parent.transform, true);
            temp.transform.localPosition += offset;
            dealDamage = temp.GetComponent<DealDamage>();
            playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[0], true, playerManager.IsOwner); //need to make by default the first animation in the array the pull out animation  
            return temp;    
        };

        currentWeaponObject = EquippingWeapon(currentWeaponSO.itemPrefab, placeWeaponLH, currentWeaponSO.offset, placeWeaponLH.transform.rotation);
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

}
