using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRun : SkillScript
{
    public override void Setting(Unit unit, Unit target) { }
    public override void Attack(Unit unit, Unit target)
    {
        if (unit.TryGetComponent<Player>(out var p))
            unit.coin += 2;
    }
    public override void End(Unit unit, Unit target) { }
}
