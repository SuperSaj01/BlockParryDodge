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
        - Handle what animations to be sent to animatorManager
        - Handle comboes
        - Handle Blocking, Parrying, Attacking
        - Hitting enemy doesnt deal damage more than once []
        - Sword touching enemy despite not swinging doesnt do damage []
    */

    private PlayerManager playerManager;

    [SerializeField] private GameObject placeWeaponLH;
    public WeaponSO currentWeaponSO;
    //private GameObject currentWeapon;

    private DealDamage dealDamage;


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
        playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[0], true, playerManager.IsOwner);// needs to be in custom logic
        
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
        currentWeaponSO = newWeapon;
        placeWeaponLH = currentWeaponSO.weapon;

    }    
    #endregion

}
