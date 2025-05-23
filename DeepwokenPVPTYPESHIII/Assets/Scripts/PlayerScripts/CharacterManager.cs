using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    protected CharacterStatHandler characterStatHandler; 
    protected AnimatorManager animatorManager;
    public CharacterNetworkManager characterNetworkManager {get; private set;}
    protected PlayerCombatManager playerCombatManager;

    [SerializeField] private GameObject damageParticlePrefab;
    
    protected virtual void Awake() 
    {
        //Assigns references by grabbing the components
        animatorManager = GetComponent<AnimatorManager>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
        playerCombatManager = GetComponent<PlayerCombatManager>();

        DontDestroyOnLoad(gameObject);
        playerCombatManager.DequipWeapon();
    }

    protected virtual void Start()
    {  
        IgnoreMyOwnColliders();    
    }

    protected virtual void Update()
    {
        UpdatePlayers();
        if(!IsOwner) return;
    }
    
 

    ///Summary of method
    ///Updates the players position, rotation, and stats and other needed variables to the network variables
    protected virtual void UpdatePlayers()
    {
        if(IsOwner)
        {
            //position
            characterNetworkManager.netPosition.Value = transform.position;
            //rotation
            characterNetworkManager.netRotation.Value = transform.rotation;
            //Stats:
            //health
            characterNetworkManager.netCurrentHealth.Value = characterStatHandler.currentHealth;
            //posture
            characterNetworkManager.netCurrentPosture.Value = characterStatHandler.currentPosture;
            
        }
        else
        {
            //movement
            transform.position = Vector3.SmoothDamp(transform.position,
            characterNetworkManager.netPosition.Value,
            ref characterNetworkManager.netPositionVel,
            characterNetworkManager.netPositionSmoothTime);
            
            //rotation
            transform.rotation = Quaternion.Slerp(transform.rotation,
                characterNetworkManager.netRotation.Value,
                characterNetworkManager.rotationSpeed);

            //Stats:
            //health
            characterStatHandler.currentHealth = characterNetworkManager.netCurrentHealth.Value;
            //posture
            // characterStatHandler.currentPosture = characterNetworkManager.netCurrentPosture.Value;
            
        }
    }

    ///Summary of method
    ///Ignores the colliders of the character controller and the damageable colliders
    void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoredColliders = new List<Collider>();

        foreach(Collider col in damageableColliders)
        {
            ignoredColliders.Add(col); //adds all the damageable colliders to the list of ignored colliders
        }
        ignoredColliders.Add(characterControllerCollider); //adds the character controller collider to the list of ignored colliders
        
        foreach(Collider col in ignoredColliders)
        {
            foreach(Collider otherCol in ignoredColliders)
            {
                Physics.IgnoreCollision(col, otherCol, true); //ignores each collider to the collision between other colliders in the list
            }
        }
    }

    public void HandleDamage(float damage)
    {
        string checkIfPlayerIsViableForDamage = playerCombatManager.ValidateDamage();
        if(checkIfPlayerIsViableForDamage == "")
        {
            characterStatHandler.TakeDamage(damage); //if the player doesnt roll or parry then damage is applied
            SpawnParticle(Color.red); // Took actual damage
        }
        else if(checkIfPlayerIsViableForDamage == "invalid")
        {
            Debug.Log("Invalid"); //does nothing
            SpawnParticle(Color.yellow); // Was in iFrames
        }
        else if(checkIfPlayerIsViableForDamage == "blocking")
        {
            Debug.Log("blocked");
            characterStatHandler.TakePostureDamage(); //if the player blocks then posture damage is applied
            SpawnParticle(new Color(0.4f, 0.26f, 0.13f)); // Brown color
        }
    }    
    private void SpawnParticle(Color color)
    {
        if (damageParticlePrefab == null) return;

          Quaternion upwardRotation = Quaternion.LookRotation(Vector3.up);

        GameObject particleObj = Instantiate(damageParticlePrefab, transform.position, upwardRotation);
        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startColor = color;
        }

        Destroy(particleObj, 2f); // Optional cleanup
    }
}
