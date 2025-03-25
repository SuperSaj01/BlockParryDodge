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
    
    protected virtual void Awake() 
    {
        animatorManager = GetComponent<AnimatorManager>();
        //playerLocomotion = GetComponent<PlayerLocomotion>();
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
    protected virtual void UpdatePlayers()
    {
        if(IsOwner)
        {
            //position
            characterNetworkManager.netPosition.Value = transform.position;
            //rotation
            characterNetworkManager.netRotation.Value = transform.rotation;
            //animation

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

            //animation


            //Stats:
            //health
            characterStatHandler.currentHealth = characterNetworkManager.netCurrentHealth.Value;
            //posture
            // characterStatHandler.currentPosture = characterNetworkManager.netCurrentPosture.Value;
            
        }
    }
    void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoredColliders = new List<Collider>();

        foreach(Collider col in damageableColliders)
        {
            ignoredColliders.Add(col);
        }
        ignoredColliders.Add(characterControllerCollider);

        foreach(Collider col in ignoredColliders)
        {
            foreach(Collider otherCol in ignoredColliders)
            {
                Physics.IgnoreCollision(col, otherCol, true);
            }
        }
    }
}
