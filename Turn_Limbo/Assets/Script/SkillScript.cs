using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript
{
    public virtual void Setting(Unit unit, Unit target) { }
    public virtual void End(Unit unit, Unit target) { }
}

public class Skill_test : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        base.End(unit, target);
        Debug.Log("Alice!");
    }
}

public class Skill_Zornhauw : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 50, 1, Unit.PropertyType.Slash));
    }
}

public class Skill_SlantCut : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == Unit.PropertyType.Slash)
            target.isAttack = false;
    }
}

public class Skill_HitAndRun : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
}

public class Skill_WarCry : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 20, 1, Unit.PropertyType.AllType));
    }
}

public class Skill_BurnAttack : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        foreach (var n in target.battleEnd)
        {
            if (n.curBuff == BuffManager.instance.debuffList[0])
            {
                n.stack += 5;
                return;
            }
        }
        target.battleEnd.Add(new Buff(BuffManager.instance.debuffList[2], 5, 1));
    }
}

public class Skill_ExplodingBlade : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.battleEnd)
        {
            if (n.curBuff == BuffManager.instance.debuffList[2])
            {
                target.turnStart.Add(new Buff(BuffManager.instance.buffList[1], 100, 10, Unit.PropertyType.AllType));
                return;
            }
        }
    }
}

public class Skill_Ox : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 20, 2, Unit.PropertyType.AllType));
    }
}

public class Skill_Eisenport : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        BuffManager.instance.buffList[1].Use(unit, 70, Unit.PropertyType.AllType);
    }
}

public class Skill_Kronhowe : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.propertyType == Unit.PropertyType.Defense && unit.TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
}

public class Skill_Hekireki_issen : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.turnStart)
        {
            if (n.curBuff == BuffManager.instance.debuffList[3])
            {
                unit.nextSkill = unit.curSkill;
                return;
            }
        }
        target.turnStart.Add(new Buff(BuffManager.instance.debuffList[3], 1, 1, Unit.PropertyType.AllType));
    }
}

public class Skill_FoamTak : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 10, 10, Unit.PropertyType.Slash));
    }
}

public class Skill_Cut : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin++;
    }
}

public class Skill_Stab : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 10, 1, Unit.PropertyType.AllType));
    }
}

public class Skill_Smashing : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == Unit.PropertyType.Defense)
            target.isAttack = false; 
    }
}

public class Skill_Defence : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.TryGetComponent<Player>(out var p)) p.addCoin += 2;
    }
}

public class Skill_Spilling : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[1], 30, 10, Unit.PropertyType.AllType));
    }
}

public class Skill_Blocking : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.turnStart.Add(new Buff(BuffManager.instance.buffList[0], 5, 1, Unit.PropertyType.AllType));
    }
}

public class Skill_ : SkillScript
{

}

