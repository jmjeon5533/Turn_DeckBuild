using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : SkillScript
{
    public override void Setting(Unit unit, Unit target) { }
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin++;
    }
}
