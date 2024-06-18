using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropertyType
{
    AllType,
    Slash,
    Hit,
    Penetrate,
    Defense
}

public abstract class Skill_Base
{
    public virtual void Setting(Unit unit, Unit target) { }
    public virtual void End(Unit unit, Unit target) { }
}

// public class Skill_Zornhauw : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         unit.buffs.SetBuff(Buffs.Key.AttackUp, value => { value.ratio = 50; value.stack = 1; });

//         // foreach (var n in target.curBuff)
//         // {
//         //     if (n.buff == DataManager.instance.debuffList["Paralysis"])
//         //     {
//         //         unit.nextSkill = unit.curSkill;
//         //         return;
//         //     }
//         // }
//         // target.curBuff.Add(new Buff(DataManager.instance.debuffList["Paralysis"], 1, 1, PropertyType.AllType));
//     }
// }

// public class Skill_SlantCut : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (target.curSkill.propertyType == PropertyType.Slash)
//             target.isAttack = false;
//     }
// }

// public class Skill_HitAndRun : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
//             p.addCoin += 2;
//     }
// }

// public class Skill_WarCry : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.buffs.SetBuff(Buffs.Key.AttackUp, x => { x.ratio = 20; x.stack = 1; });
//     }
// }

// public class Skill_BurnAttack : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if (n.buff.timing != BuffReduceTiming.battleEnd) continue;

//             if (n.buff == DataManager.instance.loadData.debuffs["Burn"])
//             {
//                 n.stack += 5;
//                 return;
//             }
//         }
//         target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffs["Burn"], 5, 1));
//     }
// }

// public class Skill_ExplodingBlade : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if (n.buff.timing != BuffReduceTiming.battleEnd) continue;

//             if (n.buff == DataManager.instance.loadData.debuffs["Burn"])
//             {
//                 target.curBuff.Add(new Buff(DataManager.instance.loadData.buffs["DefenseDown"], 100, 10, PropertyType.AllType));
//                 return;
//             }
//         }
//     }
// }

// public class Skill_Ox : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffs["AttackUp"], 20, 2, PropertyType.AllType));
//     }
// }

// public class Skill_Eisenport : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         DataManager.instance.loadData.buffs["DefenseUp"].Use(unit, 70, PropertyType.AllType);
//     }
// }

// public class Skill_Kronhowe : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (unit.usedSkill.propertyType == PropertyType.Defense && unit.TryGetComponent<Player>(out var p))
//             p.addCoin += 2;
//     }
// }

// public class Skill_Hekireki_issen : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         foreach (var n in target.curBuff)
//         {
//             if (n.buff == DataManager.instance.loadData.debuffs["Paralysis"])
//             {
//                 unit.nextSkill = unit.curSkill;
//                 return;
//             }
//         }
//         target.curBuff.Add(new Buff(DataManager.instance.loadData.debuffs["Paralysis"], 1, 1, PropertyType.AllType));
//     }
// }

// public class Skill_FoamTak : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffs["AttackUp"], 10, 10, PropertyType.Slash));
//     }
// }

// public class Skill_Cut : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         if (unit.isAttack && unit.TryGetComponent<Player>(out var p))
//             p.addCoin++;
//     }
// }

// public class Skill_Stab : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.buffs.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.attack, 0.3f, 3, Buffs.GrantOption.CurrentAttackEnd);
//     }
// }

// public class Skill_Smashing : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (target.curSkill.propertyType == PropertyType.Defense)
//             target.isAttack = false;
//     }
// }

// public class Skill_Defence : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         if (target.curSkill.propertyType == PropertyType.Hit && unit.TryGetComponent<Player>(out var p))
//             p.addCoin += 2;
//     }
// }

// public class Skill_Spilling : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         unit.buffs.AddBuff(Buffs.Key.DefenseUp, Buffs.ReduceTiming.turnEnd, 0.3f, 1);
//         // unit.curBuff.Add(new Buff(DataManager.instance.loadData.buffs["DefenseUp"], 30, 10, PropertyType.AllType));
//     }
// }

// public class Skill_Blocking : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.buffs.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.turnEnd, 0.05f, 2, Buffs.GrantOption.CurrentTurnEnd);
//     }
// }

// public class Skill_Ready : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         unit.buffs.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.attack, 50, 1, Buffs.GrantOption.CurrentTurnEnd);
//     }
// }


// public class Skill_Onguard : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (target.curSkill.propertyType == PropertyType.Slash)
//             target.buffs.AddBuff(Buffs.Key.DefenseDown, Buffs.ReduceTiming.attack, 0.2f, 1);
//     }
// }

// public class Skill_Forward : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         unit.buffs.AddBuff(Buffs.Key.DefenseDown, Buffs.ReduceTiming.turnEnd, 0.5f, 1, Buffs.GrantOption.CurrentTurnEnd);
//         if (unit.TryGetComponent<Player>(out var p))
//             p.addCoin += 3;
//     }
// }

// public class Skill_LengthCut : Skill_Base
// {
//     public override void End(Unit unit, Unit target)
//     {
//         if (unit.isAttack) unit.buffs.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.turnEnd, 0.3f, 1);
//     }
// }

// public class Skill_WidthCut : Skill_Base
// {
//     public override void Setting(Unit unit, Unit target)
//     {
//         if (unit.usedSkill.index == 14) unit.buffs.AddBuff(Buffs.Key.AttackUp, Buffs.ReduceTiming.attack, 1, 1);
//     }
// }

// public class Skill_CrossCut : Skill_Base
// {

// }

// public class Skill_ : Skill_Base
// {

// }