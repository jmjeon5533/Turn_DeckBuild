using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SkillScript
{
    public virtual void Setting(Unit unit, Unit target) { }
    public virtual void End(Unit unit, Unit target) { }
}

// public class Skill_Zornhauw : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 50, 1, PropertyType.Slash));
//     }
// }

// public class Skill_SlantCut : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (target.curSkill.propertyType == PropertyType.Slash)
//             target.isAttack = false;
//     }
// }

// public class Skill_HitAndRun : SkillScript
// {
//     public override void End(Unit unit, Unit target)
//     {
//         if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
//             p.PlusCoin(2);
//     }
// }

// public class Skill_WarCry : SkillScript
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 20, 1, PropertyType.AllType));
//     }
// }

// public class Skill_BurnAttack : SkillScript
// {
//     public override void End(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if(n.buff.timing != BuffTiming.battleEnd) continue;

//             if (n.buff == DataManager.instance.loadData.debuffList["Burn"])
//             {
//                 n.stack += 5;
//                 return;
//             }
//         }
//         target.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["Burn"], 5, 1));
//     }
// }

// public class Skill_ExplodingBlade : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if(n.buff.timing != BuffTiming.battleEnd) continue;

//             if (n.buff == DataManager.instance.loadData.debuffList["Burn"])
//             {
//                 target.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["DefenseDown"], 100, 10, PropertyType.AllType));
//                 return;
//             }
//         }
//     }
// }

// public class Skill_Ox : SkillScript
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 20, 2, PropertyType.AllType));
//     }
// }

// public class Skill_Eisenport : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         DataManager.instance.loadData.buffList["DefenseUp"].Use(unit, 70, PropertyType.AllType);
//     }
// }

// public class Skill_Kronhowe : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (unit.usedSkill.propertyType == PropertyType.Defense && unit.TryGetComponent<Player>(out var p))
//             p.PlusCoin(2);
//     }
// }

// public class Skill_Hekireki_issen : SkillScript
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if (n.buff == DataManager.instance.loadData.debuffList["Paralysis"])
//             {
//                 unit.nextSkill = unit.curSkill;
//                 return;
//             }
//         }
//         target.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
//     }
// }

// public class Skill_FoamTak : SkillScript
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 10, PropertyType.Slash));
//     }
// }

public class Skill_Cut : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(1);
    }
}

public class Skill_Stab : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 3));
    }
}

public class Skill_Smashing : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
            target.isAttack = false;
    }
}

public class Skill_Defence : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Hit && unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(2);
    }
}

public class Skill_Spilling : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["DefenseUp"], 30, 10));
    }
}

public class Skill_Blocking : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 3, 10));
    }
}

public class Skill_Ready : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 30, 1));
    }
}


public class Skill_Onguard : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash)
            target.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["AttackDown"], 20, 1));
    }
}

public class Skill_Forward : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["DefenseDown"], 50, 1));
        if (unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(3);
    }
}

public class Skill_LengthCut : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 15, 10));
    }
}

public class Skill_WidthCut : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        if (unit.usedSkill.index == 14) unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 100, 1));
    }
}

public class Skill_CrossCut : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
            target.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["AttackDown"], 90, 10, PropertyType.Defense));
        }
    }
}

public class Skill_Breath : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if ((target.curSkill.propertyType == PropertyType.Slash || target.curSkill.propertyType == PropertyType.Penetrate || target.curSkill.propertyType == PropertyType.Hit) && unit.TryGetComponent<Player>(out var p))
            p.PlusCoin(3);
    }
}

public class Skill_FirstAid : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
            int temp = Mathf.RoundToInt(unit.maxHP * 0.05f);
            unit.Recovery(temp <= 0 ? 1 : temp, true);
        }
    }
}

public class Skill_FightingSpirit : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.Recovery(Mathf.RoundToInt(unit.maxShield * 0.1f), true, false);
    }
}

public class Skill_Inkling : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
           unit.Recovery(10, false, false);
        }
    }
}

public class Skill_VitalPoint : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.shield <= Mathf.CeilToInt(target.maxShield * 0.25f))
        {
            unit.Recovery(50, false, false);
        }
    }
}

public class Skill_Hara_Kiri : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.Recovery((int)(unit.maxHP * ((float)unit.curMaxDamage * 2 / 100)), false);
    }
}

public class Skill_Crushing : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
            target.Recovery(5, false);
        }
    }
}

public class Skill_Bump : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (unit.shield <= Mathf.RoundToInt(unit.maxShield * 0.5f))
        {
            unit.Recovery(Mathf.RoundToInt(unit.shield * 0.2f), true, false);
        }
    }
}

public class Skill_Pressure : SkillScript
{
    public override void Setting(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["CoinLimit"], 0, 10));
    }
}

public class Skill_Tension : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["TrueDefenseUp"], 1, 10));
        unit.Recovery(Mathf.RoundToInt(unit.hp * 0.05f), false);
    }
}

public class Skill_Stability : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.debuffList["TrueDefenseDown"], 2, 10));
        unit.Recovery(Mathf.RoundToInt(unit.hp * 0.1f), true);
    }
}

public class Skill_BattoOjutz_Enemy : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
            target.shield = 0;
        }
    }
}

public class Skill_BattoOjutz : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Defense)
        {
            target.Recovery(20, false, false);
            if (unit.TryGetComponent<Player>(out var p))
                p.PlusCoin(3);
        }
    }
}

public class Skill_HyperSpeed : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash || target.curSkill.propertyType == PropertyType.Penetrate || target.curSkill.propertyType == PropertyType.Hit)
        {
            target.Recovery(30, false);
        }
    }
}

public class Skill_OneCut : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType != PropertyType.Slash)
        {
            unit.Recovery(Mathf.RoundToInt(unit.shield * 0.1f), true, false);
        }
    }
}

public class Skill_Determination : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash)
        {
            unit.Recovery(unit.curMaxDamage, true);
        }
    }
}

public class Skill_LengthCut_Enemy : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType != PropertyType.Slash)
        {
            unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 50, 10));
        }
    }
}

public class Skill_WidthCut_Enemy : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType != PropertyType.Slash)
        {
            target.isAttack = false;
        }
    }
}

public class Skill_Equanimity_Enemy : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType != PropertyType.Slash)
        {
            unit.Recovery(Mathf.RoundToInt(unit.maxHP * 0.25f), true);
        }
    }
}

public class Skill_Equanimity : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if (target.curSkill.propertyType == PropertyType.Slash && unit.TryGetComponent<Player>(out var p))
        {
            p.PlusCoin(3);
        }
    }
}

public class Skill_Chang : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.chainCount <= 2 && unit.TryGetComponent<Player>(out var p)){
            p.PlusCoin(3);
        }
    }
}

public class Skill_Bleed : SkillScript
{
    
}

public class Skill_Scatter : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.chainCount >= 3){
            unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 10, 2, PropertyType.Slash));
        }
    }
}

public class Skill_DeadBody : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.chainCount >= 3) target.Recovery(25, false, false);
    }
}

public class Skill_Sea : SkillScript
{
    public override void End(Unit unit, Unit target)
    {
        if(unit.chainCount >= 3){
            unit.AddBuff(BuffList.Cur, new Buff(DataManager.instance.loadData.buffList["AttackUp"], 25, 1));
        }
    }
}

public class Skill_ : SkillScript
{

}
