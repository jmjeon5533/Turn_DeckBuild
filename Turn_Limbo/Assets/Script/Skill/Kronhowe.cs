using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kronhowe : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.propertyType == Unit.PropertyType.Defense)
            unit.coin += 2;
    }
    public override void Attack(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target) { }
}
