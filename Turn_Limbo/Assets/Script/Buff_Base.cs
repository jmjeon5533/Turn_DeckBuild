// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public abstract class Buff_Base
// {
//     public Sprite buffIcon;
//     public BuffTiming timing;

//     public abstract void Use(Unit target, int stack, PropertyType type);
// }

// public class Buff_AttackUp : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if (type == PropertyType.AllType) target.attack_Drainage += (float)stack / 100;
//         else if (target.curSkill.propertyType == type) target.attack_Drainage += (float)stack / 100;
//     }
// }

// public class Buff_AttackDown : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if (type == PropertyType.AllType) target.attack_Drainage -= (float)stack / 100;
//         else if (target.curSkill.propertyType == type) target.attack_Drainage -= (float)stack / 100;
//     }
// }

// public class Buff_DefenseUp : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if (type == PropertyType.AllType) target.defense_Drainage -= (float)stack / 100;
//         else if (target.curSkill.propertyType == type) target.defense_Drainage -= (float)stack / 100;
//     }
// }

// public class Buff_DefenseDown : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if (type == PropertyType.AllType) target.defense_Drainage += (float)stack / 100;
//         else if (target.curSkill.propertyType == type) target.defense_Drainage += (float)stack / 100;
//     }
// }

// public class Buff_Burn : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         target.Damage(target.maxHP * stack / 100, Vector3.zero);
//     }
// }

// public class Buff_Paralysis : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if (target.curSkill.skillName == null) return;

//         target.InitCurSkillDamage(target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level],
//             target.curSkill.minDamage[target.skillInfo.holdSkills[target.curSkill.index].level], target.curSkill.attackCount);
//     }
// }

// public class Buff_CoinLimit : Buff_Base
// {
//     public override void Use(Unit target, int stack, PropertyType type)
//     {
//         if(!target.TryGetComponent<Player>(out var p)) return;

//         p.addCoin = 3;
//     }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public partial class Buffs
{
    //define
    public enum Key
    {
        AttackUp,
        SlashAttackUp,
        HitAttackUp,
        PenetrateAttackUp,
        AttackDown,
        DefenseUp,
        DefenseDown,
        Burn,
        Paralysis,
        ActLimit,

        End
    }

    public enum ReduceTiming
    {
        TurnStart,
        Attack,
        TurnEnd,
        BattleEnd,

        enumend,
    }

    public enum GrantOption
    {
        Immediately,
        CurrentAttackEnd,
        CurrentTurnEnd,

        enumend,
    }

    public class Value
    {
        public Key key;
        public Sprite buffIcon;
        public ReduceTiming timing;
        public PowerStack ratioStack;

        public Value(Key key, ReduceTiming timing, int power, int stack)
        {
            this.key = key;
            this.timing = timing;
            ratioStack = new PowerStack();
            ratioStack.power = power;
            ratioStack.stack = stack;
        }
    }

    public class PowerStack
    {
        public int power;
        public int stack;

        public static PowerStack operator -(PowerStack lhs, PowerStack rhs)
        {
            lhs.power -= rhs.power;
            lhs.stack -= rhs.stack;
            return lhs;
        }
    }

    private Queue<Value>[] grantQueue = new Queue<Value>[(int)GrantOption.enumend];
    public ReactiveCollection<Value> buffs = new();
    private ReactiveCollection<PowerStack> allBuffRatioStack = new();

    public event Action<Key, PowerStack> OnBuffValueChanged;

    public Unit unit;

    //method
    public Buffs()
    {
        buffs
            .ObserveAdd()
            .Buffer(Observable.EveryUpdate())
            .Where(b => b.Count > 0)
            .Select(b => (b[0].Value.key, ratio: b.Sum(v => v.Value.ratioStack.power), stack: b.Sum(v => v.Value.ratioStack.stack)))
            .Subscribe(e =>
            {
                allBuffRatioStack[(int)e.key].power += e.ratio;
                allBuffRatioStack[(int)e.key].stack += e.stack;
            });
        buffs
            .ObserveReplace()
            .Buffer(Observable.EveryUpdate())
            .Where(b => b.Count > 0)
            .Select(b => (b[0].NewValue.key, ratio: b.Sum(v => v.NewValue.ratioStack.power - v.OldValue.ratioStack.power), stack: b.Sum(v => v.NewValue.ratioStack.stack - v.OldValue.ratioStack.stack)))
            .Subscribe(e =>
            {
                allBuffRatioStack[(int)e.key].power += e.ratio;
                allBuffRatioStack[(int)e.key].stack += e.stack;
            });
        buffs
            .ObserveRemove()
            .Buffer(Observable.EveryUpdate())
            .Where(b => b.Count > 0)
            .Select(b => (b[0].Value.key, ratio: b.Sum(v => v.Value.ratioStack.power), stack: b.Sum(v => v.Value.ratioStack.stack)))
            .Subscribe(e =>
            {
                allBuffRatioStack[(int)e.key].power -= e.ratio;
                allBuffRatioStack[(int)e.key].stack -= e.stack;
            });

        for (int i = 0; i < (int)Key.End; i++) allBuffRatioStack.Add(new());
        allBuffRatioStack
            .ObserveReplace()
            .Subscribe(e => OnBuffValueChanged?.Invoke((Key)e.Index, e.NewValue - e.OldValue));

        for (int i = 0; i < (int)GrantOption.enumend; i++) grantQueue[i] = new();
    }

    public PowerStack GetBuffValue(Key key)
    {
        return allBuffRatioStack[(int)key];
    }

    public void OnGrant(GrantOption option)
    {
        if (option == GrantOption.Immediately) return;
        while (grantQueue[(int)option].TryDequeue(out var value))
            buffs.Add(value);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var stat in buffs)
            sb.Append(stat.ToString()).Append('\n');
        return sb.ToString();
    }

    public void AddBuff(Key key, ReduceTiming timing, int power, int stack, GrantOption option = GrantOption.Immediately)
    {
        if (stack == 0) return;

        var value = new Value(key, timing, power, stack);
        if (option == GrantOption.Immediately)
            buffs.Add(value);
        else
            grantQueue[(int)option].Enqueue(value);
    }

    public void UseBuff(ReduceTiming timing)
    {
        foreach (var buff in buffs)
        {
            if (buff.timing != timing) continue;

            BuffActivate(buff);

            buff.ratioStack.stack--;
            if (buff.ratioStack.stack <= 0)
                buff.ratioStack.power = 0;
        }
    }

    void BuffActivate(Value buff)
    {
        int power = buff.ratioStack.power;

        switch (buff.key)
        {
            case Key.AttackUp: unit.attack_Drainage += (float)power / 100; break;
            case Key.SlashAttackUp: if (unit.curSkill.propertyType == PropertyType.Slash) unit.attack_Drainage += (float)power / 100; break;
            case Key.HitAttackUp: if (unit.curSkill.propertyType == PropertyType.Hit) unit.attack_Drainage += (float)power / 100; break;
            case Key.PenetrateAttackUp: if (unit.curSkill.propertyType == PropertyType.Penetrate) unit.attack_Drainage += (float)power / 100; break;
            case Key.AttackDown: unit.attack_Drainage -= (float)power / 100; break;


            case Key.DefenseUp: unit.defense_Drainage -= (float)power / 100; break;
            case Key.DefenseDown: unit.defense_Drainage += (float)power / 100; break;

            case Key.Burn: unit.Damage(unit.maxHP * power / 100, Vector3.zero); break;
            case Key.Paralysis:
                if (unit.curSkill.skillName == null) return;

                unit.InitCurSkillDamage(unit.curSkill.minDamage[unit.skillInfo.holdSkills[unit.curSkill.index].level],
                unit.curSkill.minDamage[unit.skillInfo.holdSkills[unit.curSkill.index].level], unit.curSkill.attackCount);
                break;

            case Key.ActLimit: unit.GetComponent<Player>().coinLimit = true; break;
        }
        Debug.Log($"{unit.name} / {unit.curSkill.skillName} / {buff.key} / {unit.attack_Drainage}");
    }
}