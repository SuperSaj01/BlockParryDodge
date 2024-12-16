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


}
