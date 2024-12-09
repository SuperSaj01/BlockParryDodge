using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HealthSO", order = 0)]
public class HealthSO : ScriptableObject 
{
    public float maxHealth;

    public void SetValueeee(float value)
    {
        maxHealth = value;  
    }
}
