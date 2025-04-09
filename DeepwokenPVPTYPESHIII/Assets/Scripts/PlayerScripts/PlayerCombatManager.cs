using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class PlayerCombatManager : MonoBehaviour
{
    /*
    What I want this script to handle:
        - Spawning the player class (chosen)
        - Spawning the weapon (chosen)
            -can equip weapons. 
            -dynamically updates the weapon.
        - Handle what animations to be sent to animatorManager
        - Handle comboes
        - Handle Blocking, Parrying, Attacking
        - Hitting enemy doesnt deal damage more than once []
        - Sword touching enemy despite not swinging doesnt do damage []
    */

    private PlayerManager playerManager;

    [SerializeField] private GameObject placeWeaponLH;
    private WeaponSO currentWeaponSO;
    private GameObject currentWeaponObject;

    [Header("Windows")]
    private float rollWindow;
    private float parryWindow;

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
    private Queue<int> attackQueue = new Queue<int>();
    public bool isAttacking = false;
    private const int maxComboQueue = 2;
    public bool canCrit {get; private set;} = true;

    private int i = 0;



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

        wasBlocking = isBlocking;
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
        isInIFrames = true;
        Debug.Log("is rolling iframes active inshaAllah");
        Debug.Log(rollWindow);
        yield return new WaitForSeconds(rollWindow);
        isInIFrames = false;
        Debug.Log("stopped iframes");
    }

    private IEnumerator ParryWindow()
    {
        isInIFrames = true;
        Debug.Log("is Parrying frames mashaAllah");
        yield return new WaitForSeconds(parryWindow);
        canParry = true;
        isInIFrames = false;
        Debug.Log("Stopped iframes");
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
            attackQueue.Enqueue(i);
            Debug.Log("Yeye");
            i = (i + 1) % currentWeaponSO.b_aniamtions.Length;
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
        i = 0; // reset combo index after queue finishes
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
        yield return new WaitForSeconds(attackDelay);
        ulong targetId = target.OwnerClientId;
        if(isCriticalApplied)
        {
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
