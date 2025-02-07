using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{

    public Transform spawnPosition;
    public Transform spawnPosition2;
    int i = 0;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Hey");
        if(IsOwner)
        {
            SpawnPlayerToSpawnPositionServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc]
    private void SpawnPlayerToSpawnPositionServerRpc(ulong clientID)
    {
        if(IsServer)
        {
            SpawnPlayerToSpawnPositionClientRpc(clientID);
            
        }
    }

    [ClientRpc]
    private void SpawnPlayerToSpawnPositionClientRpc(ulong clientID)
    {
        if(1 ==1)
        
        if(i != 1){
            transform.position = spawnPosition.position;
            i++;
        }
        else
        {
            transform.position = spawnPosition2.position;
        }
    }
    
}
