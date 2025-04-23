using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Unity.Services.Lobbies.Models;

public class CharacterStatHandler : MonoBehaviour
{   
     

    [Header("Health")]
    [SerializeField] private float maxHealth;
    public float currentHealth = 100f;
    private float resistance = 0f;


    [Header("Posture")]
    [SerializeField] private float maxPosture = 25f;
    public float currentPosture; 

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    public float currentStamina {get; private set;}

    [Header("Windows")]
    public float rollWindow = 5f;
    public float parryWindow = 4f;

    [Header("Sliders")]
    private ISliderHandler healthHandler;
    private ISliderHandler postureHandler;
    private ISliderHandler staminaHandler;
    private List<ISliderHandler> sliderHandlers = new List<ISliderHandler>();


    void Start()
    {
        AssignStats(CharacterDatabase.GetCharacterTypeByID(1)); // setting default value
        
        AssignSliders();
        InitialiseStats();
    }

    public void UpdateSliders()
    {
        RegenStamina();
        RegenPosture();

        if(Input.GetKeyDown(KeyCode.K))
        {
            TakePostureDamage();
        }

    }

    void RegenStamina()
    {   
        // Prevent overflow
        if(currentStamina > maxStamina) currentStamina = maxStamina; 

        if(staminaHandler is null) return;
        staminaHandler.ChangeValue(currentStamina); //Update the stamina slider

        if (currentStamina < maxStamina)
        {
            // Regenerate stamina
           currentStamina += 0.2f * Time.deltaTime;            
        }
    }
    void RegenPosture()
    {
        // Prevent overflow
        if(currentPosture > maxPosture) currentPosture = maxPosture;
        postureHandler.ChangeValue(currentPosture);
        
        if (currentPosture < maxPosture)
        { 
            // Regenerate posture
            currentPosture += 0.1f * Time.deltaTime;
            if (currentPosture > maxPosture)
            {
                currentPosture = maxPosture;
            }
            
        }
    }
    public void AssignStats(CharacterSO characterType)
    {
        maxHealth = characterType.health;
        maxPosture = characterType.posture;
        maxStamina = characterType.stamina;
        resistance = characterType.resistance;
        rollWindow = characterType.rollWindow;
        parryWindow = characterType.parryWindow;
        
        AssignSliders();
        
    }
     void AssignSliders()
    {
        healthHandler = GameUIManager.instance.healthSlider.GetComponent<ISliderHandler>();
        postureHandler = GameUIManager.instance.postureSlider.GetComponent<ISliderHandler>();
        staminaHandler = GameUIManager.instance.staminaSlider.GetComponent<ISliderHandler>();

        sliderHandlers.Add(healthHandler);
        sliderHandlers.Add(postureHandler);
        sliderHandlers.Add(staminaHandler);

        //When sliders are initialised change all max values to the character types repective value
        healthHandler.SetMaxValue(maxHealth);
        postureHandler.SetMaxValue(maxPosture);
        staminaHandler.SetMaxValue(maxStamina);
        //Ensure slider current value is represented when character changes
        healthHandler.ChangeValue(currentHealth);
        postureHandler.ChangeValue(currentPosture);
        staminaHandler.ChangeValue(currentStamina);
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
            currentStamina = maxStamina;

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error Initializing Stats: {ex.Message}");
            currentHealth = 90f; // Default value
            currentPosture = 4f; // Default value
        }
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
        if(currentHealth  > 0)
        {
            float appliedDamage = damage * (1 - resistance); //resistance of character is applied to negate damage
            currentHealth -= appliedDamage;
            healthHandler.ChangeValue(currentHealth);
        }
        
        CheckIfAlive();
    }

    public void TakePostureDamage()
    {
        if(currentPosture > 0)
        {
            currentPosture -= 1; //damage is applied to posture
            postureHandler.ChangeValue(currentPosture);
        }
        CheckIfPostureBroken();
    }

    public void UseStamina()
    {
        currentStamina -= 1; 
        if(currentStamina < 0) currentStamina = 0; //Prevent stamina going below 0
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
            postureHandler.ChangeValue(0); //Hard sets posture value to 0
        }
    }

    private void Die()
    {
        WorldManager.instance.OpenDeathMenuServerRpc(); //Calls serevr to open menu for all clients
    }

}
