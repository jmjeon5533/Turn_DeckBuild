using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kronhowe : SkillScript
{
    public override void Setting(Unit @this, Unit target)
    {
        if (@this.usedSkill.propertyType == Unit.PropertyType.Defense) Controller.instance.useAbleCoin += 2;
    }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target) { }
}
