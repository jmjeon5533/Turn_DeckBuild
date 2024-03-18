using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCry : SkillScript
{
    public override void Setting(Unit @this, Unit target) { }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target)
    {
        @this.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 2, 1, Unit.PropertyType.AllType));
    }
}
