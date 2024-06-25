using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Skill_Base
{
    public virtual void Setting(Unit unit, Unit target) { }
    public virtual void End(Unit unit, Unit target) { }
}

public class Skill_Zornhauw : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.SlashAttackUp, Buffs.ReduceTiming.TurnStart, 50, 1);
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
            p.PlusCoin(2);
    }
}

public class Skill_WarCry : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 20, 1);
    }
}

public class Skill_BurnAttack : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        foreach (var n in target.buff.buffs)
        {
            if (n.key == Buffs.Key.Burn)
            {
                n.ratioStack.power += 5;
                return;
            }
        }
        target.buff.AddBuff(Buffs.Key.Burn, Buffs.ReduceTiming.BattleEnd, 5, 1);
    }
}

public class Skill_ExplodingBlade : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        foreach (var n in target.buff.buffs)
        {
            if (n.key == Buffs.Key.Burn)
            {
                target.buff.AddBuff(Buffs.Key.DefenseDown, Buffs.ReduceTiming.Attack, 100, 10);
                return;
            }
        }
    }
}

public class Skill_Ox : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 20, 2);
    }
}

public class Skill_Eisenport : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.DefenseUp, Buffs.ReduceTiming.Attack, 70, 1);
    }
}

public class Skill_Kronhowe : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.propertyType == PropertyType.Defense && unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(2);
    }
}

public class Skill_Hekireki_issen : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        // foreach (var n in target.curBuff)
        // {
        //     if (n.buff == DataManager.instance.loadData.debuffList["Paralysis"])
        //     {
        //         unit.nextSkill = unit.curSkill;
        //         return;
        //     }
        // }
        // target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
    }
}

public class Skill_FoamTak : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        //unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 10, PropertyType.Slash));
    }
}

public class Skill_Cut : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(1);
    }
}

public class Skill_Stab : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 10, 3);
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
            p.PlusCoin(2);
    }
}

public class Skill_Spilling : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.DefenseUp, Buffs.ReduceTiming.TurnStart, 30, 10);
    }
}

public class Skill_Blocking : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 3, 10);
        //stack 1?
    }
}

public class Skill_Ready : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 30, 1);
    }
}


public class Skill_Onguard : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash)
            unit.buff.AddBuff(Buffs.Key.AttackDown, Buffs.ReduceTiming.TurnStart, 20, 1);
    }
}

public class Skill_Forward : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (!unit.TryGetComponent<Player>(out var p)) return;
        
        unit.buff.AddBuff(Buffs.Key.DefenseDown, Buffs.ReduceTiming.TurnStart, 50, 1);
        p.PlusCoin(3);
    }
}

public class Skill_LengthCut : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.isAttack) unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 15, 10);    
    }
}

public class Skill_WidthCut : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        if(unit.usedSkill.index == 14) unit.buff.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.TurnStart, 100, 1); 
    }
}

public class Skill_CrossCut : Skill_Base
{

}

public class Skill_Breath : Skill_Base{
    public override void End(Unit unit, Unit target)
    {
        if (target.isAttack && unit.TryGetComponent<Player>(out var p)) p.PlusCoin(3);        
    }
}

public class Skill_FirstAid : Skill_Base{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense){
            int temp = Mathf.RoundToInt(unit.maxHP * 0.05f);
            unit.hp += temp <= 0 ? 1 : temp;
        } 
    }
}

public class Skill_FightingSpirit : Skill_Base{
    public override void End(Unit unit, Unit target)
    {
        int temp = Mathf.RoundToInt(unit.maxShield * 0.1f);
        unit.shield += temp <= 0 ? 0 : temp; 
    }
}

public class Skill_Inkling : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense){
            target.shield -= 10;
            if(target.shield < 0) target.shield = 0;
        } 
    }
}

public class Skill_VitalPoint : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (target.shield <= Mathf.CeilToInt(target.maxShield * 0.25f)){
            target.shield -= 50;
            if(target.shield < 0) target.shield = 0;
        } 
    }
}

public class Skill_Hara_Kiri : Skill_Base
{

}

public class Skill_Crushing : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense){
            target.hp -= 5;
        } 
    }
}

public class Skill_Bump : Skill_Base
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.shield <= Mathf.RoundToInt(unit.maxShield * 0.5f)){
            unit.shield += Mathf.RoundToInt(unit.shield * 0.2f);
        } 
    }
}

public class Skill_Pressure : Skill_Base
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.buff.AddBuff(Buffs.Key.ActLimit, Buffs.ReduceTiming.TurnEnd, 15, 10); 
    }
}

public class Skill_Tension : Skill_Base
{

}

public class Skill_Stability : Skill_Base
{

}

public class Skill_ : Skill_Base
{

}
