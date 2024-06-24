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
using UnityEngine;

public partial class Buffs
{
    //define
    public enum Key
    {
        AttackUp,
        AttackDown,
        DefenseUp,
        DefenseDown,
        Burn,
        Paralysis,

        End
    }

    public enum Type
    {
        All,
        Slash,
        Hit,
        Penetrate,
        Defense
    }

    public enum ReduceTiming
    {
        TurnStart,
        TurnEnd,
        BattleEnd,
        Attack,

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
        public ReduceTiming timing;
        public Type type;
        public PowerStack ratioStack;

        public Value(Key key, ReduceTiming timing, float power, int stack)
        {
            this.key = key;
            this.timing = timing;
            ratioStack.power = power;
            ratioStack.stack = stack;
        }
    }

    public class PowerStack
    {
        public float power;
        public int stack;

        public static PowerStack operator -(PowerStack lhs, PowerStack rhs)
        {
            lhs.power -= rhs.power;
            lhs.stack -= rhs.stack;
            return lhs;
        }
    }

    private Queue<Value>[] grantQueue = new Queue<Value>[(int)GrantOption.enumend];
    private ReactiveCollection<Value> buffs = new();
    private ReactiveCollection<PowerStack> allBuffRatioStack = new();

    public event Action<Key, PowerStack> OnBuffValueChanged;

    //method
    public Buffs(Unit unit)
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

    public void AddBuff(Key key, ReduceTiming timing, float ratio, int stack, Type type = Type.All, GrantOption option = GrantOption.Immediately)
    {
        if (stack == 0) return;

        var value = new Value(key, timing, ratio, stack);
        if (option == GrantOption.Immediately)
            buffs.Add(value);
        else
            grantQueue[(int)option].Enqueue(value);
    }

    public void ReduceStack(ReduceTiming timing)
    {
        foreach (var buff in buffs)
        {
            if (buff.timing != timing) continue;

            buff.ratioStack.stack--;
            if (buff.ratioStack.stack <= 0)                
                buff.ratioStack.power = 0;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var stat in buffs)
            sb.Append(stat.ToString()).Append('\n');
        return sb.ToString();
    }
}