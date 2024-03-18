using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnAttack : SkillScript
{
    public override void Setting(Unit unit, Unit target) { }
    public override void Attack(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target)
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
