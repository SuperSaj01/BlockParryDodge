using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAssassinType : CharacterType
{
    public override void Ability(PlayerManager self)
    {
        if(!canUseAbility) return;
        base.Ability(self);
        StartCoroutine(TripleHit(self));
    }

    private IEnumerator TripleHit(PlayerManager self)
    {
        self.ByPassAttack();
        yield return new WaitForSeconds(0.5f);
        self.ByPassAttack();
        yield return new WaitForSeconds(0.5f);
        self.ByPassAttack();
    }
}
