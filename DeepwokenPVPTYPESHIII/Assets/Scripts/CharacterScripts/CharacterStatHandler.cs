using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterStatHandler : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private HealthSO healthSO;

    public float currentHealth; //need a setter
    private float maxPosture; // needs to be changed to scriotable object
    public float currentPosture; //need a setter

    private float resistance = 0f;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        currentPosture = maxPosture;
        currentHealth = healthSO.maxHealth;
    }

    public void HealHealth(int healAmt)
    {
        currentHealth += healAmt;
        if(currentHealth  > healthSO.maxHealth)
        {
            currentHealth = healthSO.maxHealth;
        }
    }

    public void TakeDamage(int dmg)
    {
        if(currentHealth  > 0)
        {
            currentHealth -= dmg * (1 - resistance);
            Debug.Log(currentHealth);
        }
        
        CheckIfAlive();
        NewHealthAmt(currentHealth, true);
    }

    public void NewHealthAmt(float newHealth, bool IsOwner)
    {
        currentHealth = newHealth;
        CheckIfAlive();

        
        if(!IsOwner) return;
        playerManager.characterNetworkManager.NotifyServerOfPlayerNewHealthServerRpc(NetworkManager.Singleton.LocalClientId, newHealth);
    }

    private void CheckIfAlive()
    {
        if(currentHealth <= 0) 
        {
            Die();
            currentHealth = 0;
        }
    }

    private void Die()
    {
        Debug.Log("Peak. U ded");
    }

}
