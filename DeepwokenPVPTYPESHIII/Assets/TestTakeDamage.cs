using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTakeDamage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            Debug.Log("I will hit u ");
            player.HandleDamage(12f);
        }
    }
}
