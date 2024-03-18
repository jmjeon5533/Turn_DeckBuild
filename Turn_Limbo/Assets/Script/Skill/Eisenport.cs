using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eisenport : SkillScript
{
    public override void Setting(Unit @this, Unit target)
    {
        BuffManager.instance.buffList[1].Use(@this, 7, Unit.PropertyType.AllType);
    }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target) { }
}
