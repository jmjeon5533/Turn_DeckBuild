using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ox : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 20, 2, Unit.PropertyType.AllType));
    }
}
