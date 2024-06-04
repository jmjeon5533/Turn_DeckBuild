using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBlade : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.battleEnd)
        {
            if (n.curBuff == BuffManager.instance.debuffList[2])
            {
                target.turnStart.Add(new Buff(BuffManager.instance.buffList[1], 100, 10, Unit.PropertyType.AllType));
                return;
            }
        }
    }
}
