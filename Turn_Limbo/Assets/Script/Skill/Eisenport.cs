using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eisenport : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        BuffManager.instance.buffList[1].Use(unit, 7, Unit.PropertyType.AllType);
    }
    public override void Attack(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target) { }
}
