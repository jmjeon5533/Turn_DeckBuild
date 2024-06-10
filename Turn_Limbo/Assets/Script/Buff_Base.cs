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
        if(type == PropertyType.AllType) target.attack_Drainage += (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.attack_Drainage += (float)stack / 100;
    }
}

public class Buff_AttackDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if(type == PropertyType.AllType) target.attack_Drainage -= (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.attack_Drainage -= (float)stack / 100;
    }
}

public class Buff_DefenseUp : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if(type == PropertyType.AllType) target.defense_Drainage -= (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.defense_Drainage -= (float)stack / 100;
    }
}

public class Buff_DefenseDown : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        if(type == PropertyType.AllType) target.defense_Drainage += (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.defense_Drainage += (float)stack / 100;
    }
}

public class Buff_Burn : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.Damage(target.maxHP * stack / 100,Vector3.zero);
    }
}

public class Buff_Paralysis : Buff_Base
{
    public override void Use(Unit target, int stack, PropertyType type)
    {
        target.InitCurSkillDamage(target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level],
            target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level], target.curSkill.attackCount);
    }
}