using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRun : SkillScript
{
    public override void Setting(Unit @this, Unit target) { }
    public override void Attack(Unit @this, Unit target)
    {
        if (@this.TryGetComponent<Player>(out var p))
            Controller.instance.useAbleCoin += 2;
    }
    public override void End(Unit @this, Unit target) { }
}
