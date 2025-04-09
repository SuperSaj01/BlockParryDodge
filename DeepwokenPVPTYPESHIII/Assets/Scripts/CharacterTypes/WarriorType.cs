using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorType : CharacterType
{
    public override void Ability(PlayerManager self)
    {
        if(!canUseAbility) return;
        base.Ability(self);
        self.HealHealth(15f);
    }
}
