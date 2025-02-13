using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/WeaponsSO")]
public class WeaponSO : ItemSO
{
    //"b_" = base
    [SerializeField] public float b_Damage;
    public LayerMask b_LayerMask;

    [Tooltip("hits it takes to break posture when opponent is blocking")] 
    [SerializeField] private int b_Pressure;
    [SerializeField] public float b_SwingSpeed;
    [SerializeField] public float range;
    [SerializeField] public string[] b_aniamtions;

    [SerializeField] public Vector3 boxColliderSize;//size of the box collider
}
