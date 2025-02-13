using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DealDamage : MonoBehaviour
{

    public PlayerManager? ownPlayer;
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
    public void SetWeaponStats(PlayerManager self, float damage, float range, Vector3 boxColliderSize, LayerMask layerMask)
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
            PlayerManager target = col.GetComponent<PlayerManager>();

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
    
    void DamageTarget(PlayerManager target)
    {
        if(listOfTargets.Contains(target)) return;

        listOfTargets.Add(target);

        Debug.Log("Dealing damage to: " + target.name);

        target.TakeDamage((int)damage);

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
