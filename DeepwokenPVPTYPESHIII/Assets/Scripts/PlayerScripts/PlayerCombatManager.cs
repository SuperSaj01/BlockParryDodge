using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private GameObject placeWeaponLH;
    private WeaponSO currentWeaponSO;
    private GameObject currentWeaponObject;

    [Header("Windows")]
    private float rollWindow;
    private float parryWindow;

    [Header("Flags")]
    private bool canParry = true;
    bool isBlocking = false;
    bool isInIFrames = false;
    bool wasBlocking = false;
    float attackDelay = 0.5f;
    //private GameObject currentWeapon

    [Header("Checking Hit")]
    private DealDamage dealDamage; // accesses the damage script on the weapon
    private float multiplier = 2f;
    private bool isCriticalApplied = false;

    [Header("Queuing attacks")]
    private Queue<int> attackQueue = new Queue<int>();
    public bool isAttacking = false;
    private const int maxComboQueue = 2;
    public bool canCrit {get; private set;} = true;
    private int comboCount = 0;



    void Awake()
    {
        isAttacking = false;
        playerManager = GetComponent<PlayerManager>();
        // make current wep spawn from SO
    }

    public void HandleBlocking(bool isBlocking)
    {
        this.isBlocking = isBlocking;

        if(isBlocking && !wasBlocking && canParry)
        {
            HandleIFrames("Parrying");
        }

        wasBlocking = isBlocking; //Allows me to check if the player was previously blocking
    }

    #region IFrames
    public void HandleIFrames(string action)
    {   
        //Check if player executed perfect dodge [OPTIONAL]
        if(action == "Rolling")
        {
            StartCoroutine(RollWindow());
        }
        if(action == "Parrying")
        {
            canParry = false;
            StartCoroutine(ParryWindow());

        }
        //Disable hitbox for a few seconds. Perhaps use a coroutine
    }
    private IEnumerator RollWindow()
    {   
        //Causes IFrames during this window to be true
        isInIFrames = true;
        yield return new WaitForSeconds(rollWindow);
        isInIFrames = false;
    }

    private IEnumerator ParryWindow()
    {
        //Causes IFrames during this window to be true
        isInIFrames = true;
        yield return new WaitForSeconds(parryWindow);
        canParry = true;
        //Can parry when IFrames deactive
        isInIFrames = false;
    }

    public void InitiliaseStats(float rollWindow, float parryWindow)
    {
        this.rollWindow = rollWindow;
        this.parryWindow = parryWindow;
    }
    #endregion


    #region Dealing damage and recieving damage

     public void AttackBtnPressed(bool isCritical)
    {
        if(isCritical && canCrit) 
        {
            //If player did a critical attack and can crit then a crit is applied
            isCriticalApplied = true;
            StartCoroutine(CritReadyTimer(2f));
        }
        else
        {
            isCriticalApplied = false;
        }
        if(!HasWeaponEquipped()) return;
        if (attackQueue.Count < maxComboQueue)
        {
            attackQueue.Enqueue(comboCount);
            comboCount = (comboCount + 1) % currentWeaponSO.b_aniamtions.Length;
            //Caps combo count between 0 and 1. 
        }
        if(!isAttacking)
        {
            StartCoroutine(ProcessAttackQueue());
        }
    }

    private IEnumerator ProcessAttackQueue()
    {
        isAttacking = true;

        while (attackQueue.Count > 0)
        {
            int attackIndex = attackQueue.Dequeue();

            // Play animation and activate damage
            if(!isCriticalApplied)
            {
                playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[attackIndex], true, playerManager.IsOwner);
            }
            else
            {
                playerManager.PlayActionAnimation("Critical", true, playerManager.IsOwner);
            }
            dealDamage.DetectCollision();
            // Wait for weapon's swing speed
            
            yield return new WaitForSeconds(currentWeaponSO.b_SwingSpeed);
            isCriticalApplied = false;
        }

        isAttacking = false;
        comboCount = 0; // reset combo index after queue finishes
    }


    public void ByPassAttack()
    {
        dealDamage.DetectCollision();
    }

    public void DealDamageToTarget(PlayerManager target)
    {
        Debug.Log(isCriticalApplied);
        //tell server to handle the damage
        StartCoroutine(InitiateAttack(target, attackDelay, currentWeaponSO.b_Damage, isCriticalApplied));
        
    }

    private IEnumerator InitiateAttack(PlayerManager target, float attackDelay, float damage, bool isCriticalApplied)
    {
        //Allows opponent to parry before applying damage
        yield return new WaitForSeconds(attackDelay);
        ulong targetId = target.OwnerClientId;
        if(isCriticalApplied)
        {
            //Increases damage
            damage *= multiplier;
        }
        Debug.Log(damage);
        playerManager.characterNetworkManager.RequestDamageServerRpc(targetId, damage);
    }

    private IEnumerator CritReadyTimer(float duration)
    {
        canCrit = false; // reset first if it's already ready
        yield return new WaitForSeconds(duration);
        canCrit = true;
    }

    public string ValidateDamage()
    {
        //determines how player responded to damage
        if(isInIFrames)
        {
            return "invalid";
        }
        else if(isBlocking)
        {
            return "blocking";
        }
        else
        {
            return "";
        }
        
    }    

    #endregion

    #region Equipping/Dequipping Weapons
    private bool HasWeaponEquipped()
    {
        if(currentWeaponSO != null) return true; //Possibly add right hand in the mix later. Still need to handle player types firtst and weps  
        else return false;
    }

    public void EquipWeapon(int newWeaponID, bool IsOwner)
    {
        //Removes weapon then spawns in the new weapon and updating the stats
        DequipWeapon();
        currentWeaponSO = ItemDatabse.GetWeaponByID(newWeaponID);
        Func<GameObject, GameObject, Vector3, Quaternion, GameObject> EquippingWeapon = (wepPrefab, parent, offset, rotation) => {
            GameObject temp = Instantiate(wepPrefab, parent.transform.position, rotation); //temp is the local reference to the weapon that will be spawned. 
            temp.transform.SetParent(parent.transform, true);
            temp.transform.localPosition += offset;
            dealDamage = temp.GetComponent<DealDamage>();
            playerManager.PlayActionAnimation(currentWeaponSO.b_aniamtions[0], true, playerManager.IsOwner); //need to make by default the first animation in the array the pull out animation  
            return temp;    
        };
        
        currentWeaponObject = EquippingWeapon(currentWeaponSO.itemPrefab, placeWeaponLH, currentWeaponSO.offset, placeWeaponLH.transform.rotation);
        ChangeWeaponStats(currentWeaponSO);
        if(!IsOwner) return;
        playerManager.characterNetworkManager.NotifyServerOfInstantiatedObjectServerRpc(NetworkManager.Singleton.LocalClientId, newWeaponID);

    }
    public void DequipWeapon()
    {
        if(currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
        }
        currentWeaponSO = null;
    }

    private void ChangeWeaponStats(WeaponSO currentWeaponSO)
    {
        dealDamage.SetWeaponStats(this.GetComponent<PlayerManager>(),this, currentWeaponSO.range, currentWeaponSO.boxColliderSize, currentWeaponSO.b_LayerMask);
    }

    

    #endregion
    

}
