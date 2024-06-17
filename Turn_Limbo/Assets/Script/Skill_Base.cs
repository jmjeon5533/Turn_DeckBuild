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
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 50, 1, PropertyType.Slash));
        
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
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 20, 1, PropertyType.AllType));
    }
}

public class Skill_BurnAttack : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        foreach (var n in target.curBuff)
        {
            if(n.buff.timing != BuffTiming.battleEnd) continue;

            if (n.buff == DataManager.instance.loadData.debuffList["Burn"])
            {
                n.stack += 5;
                return;
            }
        }
        target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffList["Burn"], 5, 1));
    }
}

public class Skill_ExplodingBlade : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.curBuff)
        {
            if(n.buff.timing != BuffTiming.battleEnd) continue;

            if (n.buff == DataManager.instance.loadData.debuffList["Burn"])
            {
                target.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["DefenseDown"], 100, 10, PropertyType.AllType));
                return;
            }
        }
    }
}

public class Skill_Ox : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 20, 2, PropertyType.AllType));
    }
}

public class Skill_Eisenport : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        DataManager.instance.loadData.buffList["DefenseUp"].Use(unit, 70, PropertyType.AllType);
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
            if (n.buff == DataManager.instance.loadData.debuffList["Paralysis"])
            {
                unit.nextSkill = unit.curSkill;
                return;
            }
        }
        target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
    }
}

public class Skill_FoamTak : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 10, PropertyType.Slash));
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
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 3, PropertyType.AllType));
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
        if (target.curSkill.propertyType == PropertyType.Hit && unit.TryGetComponent<Player>(out var p)) 
            p.addCoin += 2;
    }
}

public class Skill_Spilling : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["DefenseUp"], 30, 10, PropertyType.AllType));
    }
}

public class Skill_Blocking : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 5, 1, PropertyType.AllType));
    }
}

public class Skill_Ready : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 50, 1));
    }
}


public class Skill_Onguard : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash)
            target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffList["AttackDown"], 20, 1));
    }
}

public class Skill_Forward : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.curBuff.Add(new Buff(DataManager.instance.loadData.debuffList["DefenseDown"], 50, 1));
        if (unit.TryGetComponent<Player>(out var p))
            p.addCoin += 3;
    }
}

public class Skill_LengthCut : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack) unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 30, 10, PropertyType.AllType));       
    }
}

public class Skill_WidthCut : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if(unit.usedSkill.index == 14) unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 100, 1, PropertyType.AllType));     
    }
}

public class Skill_CrossCut : Skill_Base
{

}

public class Skill_ : Skill_Base
{

}

