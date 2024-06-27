using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff_Base
{
    public Sprite buffIcon;
    public BuffTiming timing;

    public abstract void Use(Unit target, int stack, PropertyType type);
}

public class Buff_AttackUp : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (type == PropertyType.All) target.attack_Drainage += (float)stack / 100;
        else if (target.curSkill.propertyType == type) target.attack_Drainage += (float)stack / 100;
    }
}

public class Buff_AttackDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (type == PropertyType.All) target.attack_Drainage -= (float)stack / 100;
        else if (target.curSkill.propertyType == type) target.attack_Drainage -= (float)stack / 100;
    }
}

public class Buff_DefenseUp : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (type == PropertyType.All) target.defense_Drainage -= (float)stack / 100;
        else if (target.curSkill.propertyType == type) target.defense_Drainage -= (float)stack / 100;
    }
}

public class Buff_DefenseDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (type == PropertyType.All) target.defense_Drainage += (float)stack / 100;
        else if (target.curSkill.propertyType == type) target.defense_Drainage += (float)stack / 100;
    }
}

public class Buff_Burn : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.Damage(target.maxHP * stack / 100, Vector3.zero);
    }
}

public class Buff_Paralysis : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (target.curSkill.skillName == null) return;

        target.InitCurSkillDamage(target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level],
            target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level], target.curSkill.attackCount);
    }
}

public class Buff_CoinLimit : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if (!target.TryGetComponent<Player>(out var p)) return;

        p.addCoin = 3;
    }
}

public class Buff_TrueDamageUp : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.plusAttackValue += stack;
    }
}

public class Buff_TrueDefenseUp : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.plusDefenseValue += stack;
    }
}

public class Buff_TrueDamageDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.plusAttackValue -= stack;
    }
}

public class Buff_TrueDefenseDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.plusDefenseValue -= stack;
    }
}