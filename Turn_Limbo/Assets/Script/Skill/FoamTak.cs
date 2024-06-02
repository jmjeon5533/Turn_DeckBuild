using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamTak : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 10, 10, Unit.PropertyType.Slash));
    }
}
