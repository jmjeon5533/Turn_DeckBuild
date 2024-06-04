using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spilling : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[1], 30, 10, Unit.PropertyType.AllType));
    }
}
