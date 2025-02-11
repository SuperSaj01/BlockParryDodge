using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{

    public PlayerManager? ownPlayer;
    public bool isActive = true;//Instead of bool needs to be changed into coroutine by logic so syncing does not matter
    private float damage;
    private float range;
    private Vector3 boxColliderSize;
    [SerializeField] private LayerMask layerMask; 
    List<PlayerManager> listOfTargets = new List<PlayerManager>();

    int offset = 1; //offset box collider (the detection) up from floor as it spawns on floor 

    public void SetWeaponStats(PlayerManager self, float damage, float range, Vector3 boxColliderSize)
    {
        ownPlayer = self;
        this.damage = damage;
        this.range = range;
        this.boxColliderSize = boxColliderSize;
    }

    void Update()
    {
        if(ownPlayer == null) return;
        DetectCollision();

    }
    private void DetectCollision()
    {
        Vector3 center = ownPlayer.transform.position + new Vector3(0, offset, 0) + ownPlayer.transform.forward * range;
        Collider[] playerColliders = Physics.OverlapBox(center, boxColliderSize / 2, ownPlayer.transform.rotation, layerMask);
        if(playerColliders.Length == 0) return;

        PlayerManager target = playerColliders[0].GetComponent<PlayerManager>();

        if(isActive && target != null && target != ownPlayer)
        {
            Debug.Log("Hit: " + target.name);
            DamageTarget(target);
        }
    }
    
    void DamageTarget(PlayerManager target)
    {
        if(listOfTargets.Contains(target)) return;

        listOfTargets.Add(target);

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
