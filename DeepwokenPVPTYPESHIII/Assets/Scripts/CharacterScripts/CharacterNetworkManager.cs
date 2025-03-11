using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour 
{
    
    PlayerManager player;


    [Header("Position")]
    public NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(Vector3.zero,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);

    public Vector3 netPositionVel;
    public float netPositionSmoothTime;
    
    [Header("Rotation")]
    public NetworkVariable<Quaternion> netRotation = new NetworkVariable<Quaternion>(Quaternion.identity,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);

    public float rotationSpeed = 15f;    
// --------------------------------------------------
    [Header("Stats")]

    //Health
    public NetworkVariable<float> netCurrentHealth = new NetworkVariable<float>(0f,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);

     //Posture
     public NetworkVariable<float> netCurrentPosture = new NetworkVariable<float>(0f,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);

    [Header("Animation")]
    public NetworkVariable<float> netMoveAmount = new NetworkVariable<float>(0,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> netIsRunning = new NetworkVariable<bool>(false,
     NetworkVariableReadPermission.Everyone,
     NetworkVariableWritePermission.Owner);
    
    
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        var _ = ItemDatabse.GetWeaponByID(-1); //dummy call to initialise the database
    }

    [ServerRpc] //invoked by the client to be recieved by the server/host
    public void NotifyServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool isInteracting) // when root motion is added, add here and all other functions 
    {
        if(IsServer)
        {
            NotifyClientsOfActionAnimationClientRpc(clientID, animationID, isInteracting);
        }
    }

    [ClientRpc] //invoked by the server/host to be recieved to all clients
    private void NotifyClientsOfActionAnimationClientRpc(ulong clientID, string animationID, bool isInteracting)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PlayActionAnimation(animationID, isInteracting);
        }
    }
     
    private void PlayActionAnimation(string animationID, bool isInteracting)
    {
        player.PlayActionAnimation(animationID, isInteracting, false);
    }

    [ServerRpc] //invoked by the client to be recieved by the server/host
    public void NotifyServerOfInstantiatedObjectServerRpc(ulong clientID, int weaponID)
    {
        if(IsServer)
        {
            NotifyClientsOfInstantiatedObjectClientRpc(clientID, weaponID);
        }
    }
    [ClientRpc] //invoked by the server/host to be recieved to all clients
    private void NotifyClientsOfInstantiatedObjectClientRpc(ulong clientID, int weaponID)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            if(weaponID != -1)
            {
                player.EquipWeapon(weaponID, false);
            }
            else
            {
                player.DequipWeapon();
            }
        }
    }

    [ServerRpc]
    public void RequestDamageServerRpc(ulong targetId, ulong clientID, float damage)
    {
        Debug.Log($"Server received RequestDamageServerRpc from {clientID}");
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(targetId))
            { 
                Debug.Log("Target not found");
                return;
            }  // Validate target exists

            PlayerManager targetPlayer = NetworkManager.Singleton.ConnectedClients[targetId].PlayerObject.GetComponent<PlayerManager>();
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(damage);
            }
        
    }
    [ClientRpc]
    private void HandleDamageClientRpc()
    {

    }
    [ClientRpc]
    private void NotifyClientsOfPlayerNewHealthClientRpc(ulong clientID, float newHealth)
    {
        if(IsServer)
        {
            if(clientID != NetworkManager.Singleton.LocalClientId)
            {
                player.SetNewHealthAmt(netCurrentHealth.Value, false);
            }
        }
        
    }
    
}
