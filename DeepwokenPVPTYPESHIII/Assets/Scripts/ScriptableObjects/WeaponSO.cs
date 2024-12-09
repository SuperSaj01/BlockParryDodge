using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponsSO")]
public class WeaponSO : ScriptableObject
{
    [SerializeField] private string wepName;
    public GameObject wepPrefab;

    //"b_" = base
    [SerializeField] private float b_Damage;

    [Tooltip("hits it takes to break posture when opponent is blocking")]
    [SerializeField] private int b_Pressure;
    [SerializeField] private float b_SwingSpeed;
    [SerializeField] private float range;
    [SerializeField] private float scale; //In Question? Good idea or nah 


 
 

}
