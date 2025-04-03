using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class CharacterStatHandler : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private GameObject healthSlider;
    [SerializeField] private GameObject postureSlider;
     

    [Header("Health")]
    private float maxHealth;
    public float currentHealth = 100f;
    private float resistance = 0f;
    private int regenAmount = 5;
    private int timeUntilRegen = 2;
    private int lastTimeSinceDamage;


    [Header("Posture")]
    private float maxPosture = 25f;
    public float currentPosture; 

    [Header("Stamina")]
    private float maxStamina;
    public float stamina;

    [Header("Windows")]
    public float rollWindow = 5f;
    public float parryWindow = 4f;

    [Header("Sliders")]
    private ISliderHandler healthHandler;
    private ISliderHandler postureHandler;
    private List<ISliderHandler> sliderHandlers = new List<ISliderHandler>();

    void Awake()
    {

    }

    void Start()
    {
        AssignStats(CharacterDatabase.GetCharacterTypeByID(1)); // setting default value
        
        AssignSliders();
    }

    private void Update()
    {
        

    }

    public void AssignStats(CharacterSO characterType)
    {
        maxHealth = characterType.health;
        maxPosture = characterType.posture;
        resistance = characterType.resistance;
        rollWindow = characterType.rollWindow;
        parryWindow = characterType.parryWindow;
        maxStamina = characterType.stamina;
 
        InitialiseStats();
    }

    private void InitialiseStats()
    {
        

        try
        {
            if (maxHealth <= 0)
            {
                throw new System.Exception("Default character was not assigned");
            }
            
            currentHealth = maxHealth;
            currentPosture = maxPosture;

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error Initializing Stats: {ex.Message}");
            currentHealth = 90f; // Default value
            currentPosture = 4f; // Default value
        }
    }


    void AssignSliders()
    {
        healthHandler = GameUIManager.instance.healthSlider.GetComponent<ISliderHandler>();
        postureHandler = GameUIManager.instance.postureSlider.GetComponent<ISliderHandler>();

        sliderHandlers.Add(healthHandler);
        sliderHandlers.Add(postureHandler);

        healthHandler.SetMaxValue(maxHealth);
        postureHandler.SetMaxValue(maxPosture);
    }

    public void HealHealth(int healAmt)
    {
        currentHealth += healAmt;
        if(currentHealth  > maxHealth)
        {
            currentHealth = maxHealth; //Stops the health from going beyond max
        }
        healthHandler.ChangeValue(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log(this.name + damage + "yess");
        if(currentHealth  > 0)
        {
            float appliedDamage = damage * (1 - resistance); //resistance of character is applied to negate damage
            currentHealth -= appliedDamage;
            healthHandler.ChangeValue(currentHealth);
            lastTimeSinceDamage = (int)Time.time;
        }
        
        CheckIfAlive();
    }

    public void TakePostureDamage(float postureDamage)
    {
        if(currentPosture > 0)
        {
            currentPosture -= postureDamage; //damage is applied to posture
            postureHandler.ChangeValue(currentPosture);
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
            postureHandler.ChangeValue(0);
        }
    }

    private void Die()
    {
        WorldManager.instance.OpenDeathMenuServerRpc();
    }

}
