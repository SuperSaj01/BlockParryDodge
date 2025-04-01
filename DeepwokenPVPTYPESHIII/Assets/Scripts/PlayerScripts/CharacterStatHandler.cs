using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class CharacterStatHandler : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private HealthSO healthSO;

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

    [Header("Windows")]
    public float rollWindow = 5f;
    public float parryWindow = 4f;

    [Header("Sliders")]
    private ISliderHandler healthHandler;
    private ISliderHandler postureHandler;
    private List<ISliderHandler> sliderHandlers = new List<ISliderHandler>();

    private void Awake()
    {
        currentPosture = maxPosture;
        //currentHealth = healthSO.maxHealth;
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
        if(Input.GetKeyDown(KeyCode.J))
        {
            HealHealth(5);
            Debug.Log("Hey");
        }

    }


    void AssignSliders()
    {
        healthHandler = healthSlider.GetComponent<ISliderHandler>();
        postureHandler = postureSlider.GetComponent<ISliderHandler>();

        sliderHandlers.Add(healthHandler);
        sliderHandlers.Add(postureHandler);

        healthHandler.SetMaxValue(healthSO.maxHealth);
        postureHandler.SetMaxValue(maxPosture);
    }

    public void HealHealth(int healAmt)
    {
        currentHealth += healAmt;
        if(currentHealth  > healthSO.maxHealth)
        {
            currentHealth = healthSO.maxHealth; //Stops the health from going beyond max
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
