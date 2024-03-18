using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis : Buff_Base
{
    public override void Use(Unit target, int stack, Unit.PropertyType type)
    {
        target.InitCurSkillDamage(target.curSkill.minDamage, target.curSkill.minDamage, target.curSkill.attackCount);
    }
}
