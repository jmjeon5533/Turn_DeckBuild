using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRun : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
}
