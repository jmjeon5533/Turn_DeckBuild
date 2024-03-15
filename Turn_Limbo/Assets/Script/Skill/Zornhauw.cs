using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zornhauw : SkillScript
{
    public override void Setting(Unit @this, Unit target) { }
    public override void Attack(Unit @this, Unit target) { }
    public override void End(Unit @this, Unit target)
    {
        Debug.Log($"{BuffManager.instance.buffList[0] == null}");
        @this.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 5, true));
    }
}
