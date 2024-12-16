using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private string name;
    public GameObject itenPrefab;

 
}
