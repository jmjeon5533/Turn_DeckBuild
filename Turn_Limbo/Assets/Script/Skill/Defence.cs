using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.TryGetComponent<Player>(out var p)) p.addCoin += 2;
    }
}
