using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{

    public PlayerManager? ownPlayer;
    private PlayerCombatManager playerCombatManager;
    public bool isActive = true;//Instead of bool needs to be changed into coroutine by logic so syncing does not matter
    private float damage = 4;
    private float range;
    private Vector3 boxColliderSize;
    [SerializeField] private LayerMask layerMask;
    List<CharacterManager> listOfTargets = new List<CharacterManager>();

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

            if (target != null && target != ownPlayer) // Ignore self
            {
                Debug.Log("Hit: " + target.name);
                ValidateTarget(target);
                return; // Stop after hitting the first valid target
            }
        }
    }



    void ValidateTarget(CharacterManager target)

    {
        if(listOfTargets.Contains(target)) return;

        listOfTargets.Add(target);

        playerCombatManager.DealDamageToTarget(target);
    
        listOfTargets.Remove(target);
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
