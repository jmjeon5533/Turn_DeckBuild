using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hekireki_issen : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.turnStart)
        {
            if (n.curBuff == BuffManager.instance.debuffList[3])
            {
                unit.nextSkill = unit.curSkill;
                return;
            }
        }
        target.turnStart.Add(new Buff(BuffManager.instance.debuffList[3], 1, 1, Unit.PropertyType.AllType));
    }
    public override void End(Unit unit, Unit target) { }
}
