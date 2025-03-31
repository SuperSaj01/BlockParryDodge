using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class CharacterStatHandler : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private HealthSO healthSO;

    [SerializeField] private GameObject healthSlider;
    private ISliderHandler healthHandler;
    [SerializeField] private GameObject postureSlider;
     private ISliderHandler postureHandler;

    [Header("Health")]
    private float maxHealth;
    public float currentHealth; 
    private float resistance = 0f;
    private int regenAmount = 2;
    private int timeUntilRegen = 10;
    private int lastTimeSinceDamage;


    [Header("Posture")]
    private float maxPosture; 
    public float currentPosture; 

    [Header("Windows")]
    public float rollWindow = 5f;
    public float parryWindow = 4f;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        currentPosture = maxPosture;
        currentHealth = healthSO.maxHealth;
    }

    void Start()
    {
        AssignSliders();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(5);
        }

        if(Time.time - lastTimeSinceDamage >= timeUntilRegen)
        {
            HealHealth(regenAmount);
        }

    }


    void AssignSliders()
    {
        healthHandler = healthSlider.GetComponent<ISliderHandler>();
        postureHandler = postureSlider.GetComponent<ISliderHandler>();

        Instantiate(healthSlider);
        Instantiate(postureSlider);
    }

    public void HealHealth(int healAmt)
    {
        currentHealth += healAmt * Time.deltaTime;
        if(currentHealth  > healthSO.maxHealth)
        {
            currentHealth = healthSO.maxHealth; //Stops the health from going beyond max
        }
        healthHandler.IncreaseValue(healAmt);
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth  > 0)
        {
            float appliedDamage = damage * (1 - resistance); //resistance of character is applied to negate damage
            currentHealth -= appliedDamage;
            healthHandler.ReduceValue(appliedDamage);
            lastTimeSinceDamage = (int)Time.time;
        }
        
        CheckIfAlive();
    }

    public void TakePostureDamage(float postureDamage)
    {
        if(currentPosture > 0)
        {
            currentPosture -= postureDamage; //damage is applied to posture
            postureHandler.ReduceValue(postureDamage);
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
            postureHandler.ReduceValue(100f);
        }
    }

    private void Die()
    {
        Debug.Log("Peak. U ded");
    }

}
