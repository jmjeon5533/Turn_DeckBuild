using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBlade : SkillScript
{
    public override void Setting(Unit @this, Unit target)
    {
        foreach (var n in target.battleEnd)
        {
            if (n.curBuff == BuffManager.instance.debuffList[0])
            {
                target.turnStart.Add(new Buff( BuffManager.instance.buffList[1], -10, 10, Unit.PropertyType.AllType));
                return;
            }
        }
    }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target) { }
}
