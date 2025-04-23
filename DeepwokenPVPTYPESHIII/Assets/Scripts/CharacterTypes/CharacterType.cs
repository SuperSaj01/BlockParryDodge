using System.Collections;
using UnityEngine;

public class CharacterType : MonoBehaviour
{
    public bool canUseAbility {get; private set;} = true;
    public virtual void Ability(PlayerManager playerManager)
    {
        if(!canUseAbility) return;
        StartCoroutine(AbilityReadyTimer(5f));
    }
    private IEnumerator AbilityReadyTimer(float duration)
    {
        canUseAbility = false; // reset first if it's already ready
        yield return new WaitForSeconds(duration);
        canUseAbility = true;
    }
}
