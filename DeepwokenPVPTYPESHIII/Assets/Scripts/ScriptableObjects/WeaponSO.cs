using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/WeaponsSO")]
public class WeaponSO : ItemSO
{
    //"b_" = base
    [SerializeField] public float b_Damage { get; private set; }

    [Tooltip("hits it takes to break posture when opponent is blocking")]
    [SerializeField] private int b_Pressure;
    [SerializeField] private float b_SwingSpeed;
    [SerializeField] private float range;
    [SerializeField] public string[] b_aniamtions;

    [SerializeField] public GameObject damageCollider; //this is subject to change perhaps. 
    //I am using a collider to check if a player is being hit. Then deal damage script will be on the damage collider. 
    //This allows me to perform perfect dodges. 

}
