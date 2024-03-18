using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spilling : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        //Debug.Log($"{target.name} {target.curSkill.skillName} {target.curSkill.propertyType}");
        if (target.curSkill.propertyType == Unit.PropertyType.Slash)
            target.isAttack = false; 
    }
    public override void Attack(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target) { }
}
