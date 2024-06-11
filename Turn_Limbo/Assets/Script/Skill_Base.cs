using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill_Base
{
    public virtual void Setting(Unit unit, Unit target) { }
    public virtual void End(Unit unit, Unit target) { }
}

public class Skill_Zornhauw : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 50, 1, PropertyType.Slash));
        
        // foreach (var n in target.curBuff)
        // {
        //     if (n.buff == DataManager.instance.debuffList["Paralysis"])
        //     {
        //         unit.nextSkill = unit.curSkill;
        //         return;
        //     }
        // }
        // target.curBuff.Add(new Buff(DataManager.instance.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
    }
}

public class Skill_SlantCut : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash)
            target.isAttack = false;
    }
}

public class Skill_HitAndRun : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
}

public class Skill_WarCry : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 20, 1, PropertyType.AllType));
    }
}

public class Skill_BurnAttack : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        foreach (var n in target.curBuff)
        {
            if(n.buff.timing != BuffTiming.battleEnd) continue;

            if (n.buff == DataManager.instance.debuffList["Burn"])
            {
                n.stack += 5;
                return;
            }
        }
        target.curBuff.Add(new Buff(DataManager.instance.debuffList["Burn"], 5, 1));
    }
}

public class Skill_ExplodingBlade : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.curBuff)
        {
            if(n.buff.timing != BuffTiming.battleEnd) continue;

            if (n.buff == DataManager.instance.debuffList["Burn"])
            {
                target.curBuff.Add(new Buff(DataManager.instance.buffList["DefenseDown"], 100, 10, PropertyType.AllType));
                return;
            }
        }
    }
}

public class Skill_Ox : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 20, 2, PropertyType.AllType));
    }
}

public class Skill_Eisenport : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        DataManager.instance.buffList["DefenseUp"].Use(unit, 70, PropertyType.AllType);
    }
}

public class Skill_Kronhowe : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.propertyType == PropertyType.Defense && unit.TryGetComponent<Player>(out var p))
            p.addCoin += 2;
    }
}

public class Skill_Hekireki_issen : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.curBuff)
        {
            if (n.buff == DataManager.instance.debuffList["Paralysis"])
            {
                unit.nextSkill = unit.curSkill;
                return;
            }
        }
        target.curBuff.Add(new Buff(DataManager.instance.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
    }
}

public class Skill_FoamTak : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 10, 10, PropertyType.Slash));
    }
}

public class Skill_Cut : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.addCoin++;
    }
}

public class Skill_Stab : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 10, 1, PropertyType.AllType));
    }
}

public class Skill_Smashing : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
            target.isAttack = false; 
    }
}

public class Skill_Defence : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.TryGetComponent<Player>(out var p)) p.addCoin += 2;
    }
}

public class Skill_Spilling : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["DefenseUp"], 30, 10, PropertyType.AllType));
    }
}

public class Skill_Blocking : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.buffList["AttackUp"], 5, 1, PropertyType.AllType));
    }
}

public class Skill_ : Skill_Base
{

}

