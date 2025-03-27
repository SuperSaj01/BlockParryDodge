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


    public float rollWindow = 5f;
    public float parryWindow = 4f;
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
            currentHealth = healthSO.maxHealth; //Stops the health from going beyond max
        }
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth  > 0)
        {
            currentHealth -= damage * (1 - resistance); //resistance of character is applied to negate damage
            Debug.Log(currentHealth);
        }
        
        CheckIfAlive();
    }

    public void TakePostureDamage(float postureDamage)
    {
        if(currentPosture > 0)
        {
            currentPosture -= postureDamage; //damage is applied to posture
        }
        CheckIfPostureBroken();
    }

    private void CheckIfAlive()
    {
        if(currentHealth <= 0) 
        {
            Die();
            currentHealth = 0;
        }
    }

    private void CheckIfPostureBroken()
    {
        if(currentPosture <= 0)
        {
            //break posture
        }
    }

    private void Die()
    {
        Debug.Log("Peak. U ded");
    }

}
