using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCry : SkillScript
{
    public override void Setting(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 20, 1, Unit.PropertyType.AllType));
    }
}
