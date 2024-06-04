using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smashing : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == Unit.PropertyType.Defense)
            target.isAttack = false; 
    }
}
