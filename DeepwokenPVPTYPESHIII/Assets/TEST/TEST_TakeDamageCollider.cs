using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_TakeDamageCollider : MonoBehaviour
{

    int damage = 20;

    void OnTriggerEnter(Collider col)
    {
        
        if(col.CompareTag("HitBox"))
        {
            Debug.Log("A hitbox has been hit");
            PlayerManager opponentPlayer = col.gameObject.GetComponent<PlayerManager>();
            Hit(opponentPlayer);
            Debug.Log("hit player");
        }
    }    

    void Hit(PlayerManager opponentPlayer)
    {
        opponentPlayer.TakeDamage(damage);
        Debug.Log("player has taken " + damage + " damage");
    }
    
}
