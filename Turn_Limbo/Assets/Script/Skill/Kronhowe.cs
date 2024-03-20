using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kronhowe : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.propertyType == Unit.PropertyType.Defense && TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
    public override void End(Unit unit, Unit target) { }
}
