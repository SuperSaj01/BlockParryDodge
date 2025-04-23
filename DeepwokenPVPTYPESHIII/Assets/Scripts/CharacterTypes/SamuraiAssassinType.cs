using System.Collections;
using UnityEngine;

public class SamuraiAssassinType : CharacterType
{
    public override void Ability(PlayerManager self)
    {
        if(!canUseAbility) return; //prevents ability to be used if flag is false
        base.Ability(self);
        StartCoroutine(TripleHit(self));
    }

    private IEnumerator TripleHit(PlayerManager self)
    {
        
        self.ByPassAttack();
        yield return new WaitForSeconds(0.5f); //Attacks multiple times after a delay
        self.ByPassAttack();
        yield return new WaitForSeconds(0.5f);
        self.ByPassAttack();
    }
}
