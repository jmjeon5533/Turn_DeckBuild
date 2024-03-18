using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hekireki_issen : SkillScript
{
    public override void Setting(Unit @this, Unit target) { }
    public override void Attack(Unit @this, Unit target)
    {
        // foreach (var n in target.turnStart)
        // {
        //     if (n.curBuff == BuffManager.instance.debuffList[1])
        //     {
        //         target.curSkill.insertImage = UIManager.instance.AddImage(target.curSkill.icon, target.requestUIParent);
        //         target.attackRequest.Peek() = target.curSkill;
        //         return;
        //     }
        // }
    }
    public override void End(Unit @this, Unit target) { }
}
