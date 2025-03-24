using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : NetworkBehaviour
{

    public PlayerManager? ownPlayer;
    private PlayerCombatManager playerCombatManager;
    public bool isActive = true;//Instead of bool needs to be changed into coroutine by logic so syncing does not matter
    private float damage = 4;
    private float range;
    private Vector3 boxColliderSize;
    [SerializeField] private LayerMask layerMask;
    List<PlayerManager> listOfTargets = new List<PlayerManager>();

    int offset = 1; //offset box collider (the detection) up from floor as it spawns on floor 

    void Awake()
    {

    }
    public void SetWeaponStats(PlayerManager self, PlayerCombatManager playerCombatManager, float damage, float range, Vector3 boxColliderSize, LayerMask layerMask)
    {
        ownPlayer = self;
        this.damage = damage;
        this.range = range;
        this.boxColliderSize = boxColliderSize;
        this.layerMask = layerMask;
    }

    void Update()
    {
    }
    public void DetectCollision()
    {
        if(ownPlayer == null) return;
        Vector3 center = ownPlayer.transform.position + new Vector3(0, offset, 0) + ownPlayer.transform.forward * range;
        Collider[] playerColliders = Physics.OverlapBox(center, boxColliderSize / 2, ownPlayer.transform.rotation, layerMask);
        if (playerColliders.Length == 0) return;

        Debug.Log("Hit: " + playerColliders.Length + " targets");
        foreach (Collider col in playerColliders) // Loop through all detected colliders
        {
            CharacterManager target = col.GetComponent<CharacterManager>();

            if(target != null ) Debug.Log("not null");
            if(target == null) Debug.Log("null");

            Debug.Log(target);

            if(target != null ) Debug.Log("not null");
            if(target == null) Debug.Log("null");

            Debug.Log(target);

            if(target != null ) Debug.Log("not null");
            if(target == null) Debug.Log("null");

            Debug.Log(target);

            if (target != null && target != ownPlayer) // Ignore self
            {
                Debug.Log("Hit: " + target.name);
                DamageTarget(target);
                return; // Stop after hitting the first valid target
            }
        }
    }

   /* private void OnTriggerEnter(Collider other)
    {
        if(isActive)
        {
            PlayerManager target = other.GetComponent<PlayerManager>();
            if(target != null && target != ownPlayer)
            {
                Debug.Log("Hit: " + target.name);
                DamageTarget(target);
            }
        }
    } */
    
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
    void DamageTarget(PlayerManager target)
=======
    void ValidateTarget(CharacterManager target)
>>>>>>> Stashed changes
=======
    void DamageTarget(PlayerManager target)
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
=======
    void DamageTarget(PlayerManager target)
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
    {
        if(listOfTargets.Contains(target.GetComponent<PlayerManager>())) return;

<<<<<<< Updated upstream
        listOfTargets.Add(target);

        Debug.Log("Dealing damage to: " + target.name);
        ulong targetId = target.ClientID;
        Debug.Log(targetId);
        ulong ownId = ownPlayer.ClientID;
        
<<<<<<< HEAD
<<<<<<< HEAD
=======
        listOfTargets.Add(target.GetComponent<PlayerManager>());
        /*
>>>>>>> Stashed changes
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        Debug.Log($"Is {gameObject.name} NetworkObject spawned? {NetworkObject.IsSpawned}");
        Debug.Log("My ClientId: " + ownId);
        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} is calling ServerRpc!");
        Debug.Log($"Client Connected: {NetworkManager.Singleton.IsConnectedClient}");

        Debug.Log($"Calling ServerRpc on {ownId}");
        Debug.Log("Client is calling RequestDamageServerRpc");
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
        ownPlayer.characterNetworkManager.RequestDamageServerRpc(targetId, ownId, damage);

        listOfTargets.Remove(target);
=======
        playerCombatManager.DealDamageToTarget(target);
        */
        playerCombatManager.DealDamageToTarget(target.GetComponent<PlayerManager>());
        listOfTargets.Remove(target.GetComponent<PlayerManager>());
>>>>>>> Stashed changes
=======
=======
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
        ownPlayer.characterNetworkManager.RequestDamageServerRpc(targetId, ownId, damage);

        listOfTargets.Remove(target);
>>>>>>> parent of 97aa660 (Added States/ fixed damadging yippee)
    }

    

    private void OnDrawGizmos()
    {
        if(ownPlayer == null) return;
        Gizmos.color = Color.red;
        Vector3 boxCenter = ownPlayer.transform.position + new Vector3(0, offset, 0) + ownPlayer.transform.forward * range;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, ownPlayer.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxColliderSize);
    }
}
