using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float TakeDamage(int dmg)
    {
        if(currentHealth  > 0)
        {
            currentHealth -= dmg * (1 - resistance);
            Debug.Log(currentHealth);
        }
        
        CheckIfAlive();
        return currentHealth;
    }

    public void NewHealthAmt(float newHealth)
    {
        currentHealth = newHealth;
        CheckIfAlive();
    }

    private void CheckIfAlive()
    {
        if(currentHealth <= 0) Die();
        currentHealth = 0;
    }

    private void Die()
    {
        Debug.Log("Peak. U ded");
    }

}
