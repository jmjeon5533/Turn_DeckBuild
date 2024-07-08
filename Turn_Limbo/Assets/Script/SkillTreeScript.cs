using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTreeScript
{
    public abstract void Use(PlusStats plusStats, string value, Unit target = null);
}

public class SkillTree_HP : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.maxHP += int.Parse(value);
        Debug.Log(plusStats.maxHP + " / " + int.Parse(value));
    }
}

public class SkillTree_Shield : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.maxShield += int.Parse(value);
        Debug.Log(plusStats.maxShield + " / " + int.Parse(value));
    }
}

public class SkillTree_AttackValue : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.plusAttackValue += int.Parse(value);
        Debug.Log(plusStats.plusAttackValue + " / " + int.Parse(value));
    }
}

public class SkillTree_DefenseValue : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.plusDefenseValue += int.Parse(value);
    }
}

public class SkillTree_AttackDrainage : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.attack_Drainage += float.Parse(value);
    }
}

public class SkillTree_DefenseDrainage : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.defense_Drainage += float.Parse(value);
    }
}

public class SkillTree_AddCoin : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        plusStats.addCoin += int.Parse(value);
    }
}

public class SkillTree_ : SkillTreeScript
{
    public override void Use(PlusStats plusStats, string value, Unit target = null)
    {
        
    }
}
