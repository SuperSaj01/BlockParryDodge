using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSpawnPointSO", menuName = "ScriptableObjects/SpawnPointSO")]
public class SpawnPointSO : ScriptableObject
{
    public List<Vector3> spawnPoints = new List<Vector3>();

    
}
