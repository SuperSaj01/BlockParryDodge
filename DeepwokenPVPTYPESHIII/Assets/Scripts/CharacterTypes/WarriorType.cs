public class WarriorType : CharacterType
{
    public override void Ability(PlayerManager self)
    {
        if(!canUseAbility) return; //prevents ability to be used if flag is false
        base.Ability(self);
        self.HealHealth(15f); //Heals player
    }
}
