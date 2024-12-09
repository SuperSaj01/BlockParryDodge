using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmutableSO : MonoBehaviour
{
    [SerializeField] List<RestoringScriptableObjects> scriptableObjects;

    private void Awake() {
        foreach (RestoringScriptableObjects scriptableObject in scriptableObjects)
        {   
            scriptableObject.InitialiseValue();
        }
    }
}

[System.Serializable]
public class RestoringScriptableObjects
{
    [SerializeField] HealthSO SO;
    [SerializeField] float value  = 0f;

    public void InitialiseValue()
    {
        SO.SetValueeee(value);
    }
    
}
