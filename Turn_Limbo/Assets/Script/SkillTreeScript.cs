using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTreeScript
{
    public abstract void Use(PlusStats plusStats, string value);
}

public class SkillTree_HP : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.maxHP += int.Parse(value);
    }
}

public class SkillTree_Shield : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.maxShield += int.Parse(value);
    }
}

public class SkillTree_AttackValue : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.plusAttackValue += int.Parse(value);
    }
}

public class SkillTree_DefenseValue : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.plusDefenseValue += int.Parse(value);
    }
}

public class SkillTree_AttackDrainage : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.attack_Drainage += float.Parse(value);
    }
}

public class SkillTree_DefenseDrainage : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.defense_Drainage += float.Parse(value);
    }
}

public class SkillTree_AddCoin : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        plusStats.addCoin += int.Parse(value);
    }
}

public class SkillTree_PBuff : SkillTreeScript{
    public override void Use(PlusStats plusStats, string value){
        var splitExplain = value.Split('&');
        foreach(var n in splitExplain){
            
        }
    }
}

public class SkillTree_PLoopBuff : SkillTreeScript{
    public override void Use(PlusStats plusStats, string value){

    }
}

public class SkillTree_ : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value)
    {
        
    }
}
