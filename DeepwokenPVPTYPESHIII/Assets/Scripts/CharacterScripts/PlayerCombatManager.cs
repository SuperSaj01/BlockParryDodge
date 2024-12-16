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

    public WeaponSO currentWeaponSO;
    //private GameObject currentWeapon;

    public GameObject currentWeapon; //temp

    private DealDamage dealDamage;


    [Header("Weapons Stats")]
    private float damage;

    void Awake()
    {
        
        playerManager = GetComponent<PlayerManager>();
        // make current wep spawn from SO

        dealDamage = currentWeapon.GetComponent<DealDamage>();
        
     
    }

    public void AttackBtnPressed()
    {
        
        playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[0], true, playerManager.IsOwner);// needs to be in custom logic
    }


}
