using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAttack : SkillScript
{
    public override void Setting(Unit @this, Unit target) { }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target)
    {
        foreach (var n in target.battleEnd)
        {
            if (n.curBuff == BuffManager.instance.debuffList[0])
            {
                n.stack += 5;
                return;
            }
        }
        target.battleEnd.Add(new Buff(BuffManager.instance.debuffList[0], 5, 1));
    }
}
