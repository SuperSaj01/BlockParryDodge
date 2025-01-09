using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{

    public PlayerManager? ownPlayer;
    public bool isActive = true;//Instead of bool needs to be changed into coroutine by logic so syncing does not matter
    public float damage;
    List<PlayerManager> listOfTargets = new List<PlayerManager>();

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager target = other.GetComponent<PlayerManager>();

        if(isActive && target != null && target != ownPlayer)
        {
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

}
