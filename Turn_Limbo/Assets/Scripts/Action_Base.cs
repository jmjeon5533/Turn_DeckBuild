using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    None,
    Slash,
    Hit,
    Penetrate,
    Defense
}

public class Action_Base
{
    protected ActionInfo skillInfo;
    protected Unit caster;
    protected int actionLv;
    protected Unit target;

    public void Initialize(Unit caster, int actionLv, Unit target)
    {
        this.caster = caster;
        this.actionLv = actionLv;
        this.target = target;
    }

    public virtual void OnAttackStart()
    {

    }

    public virtual int GetActionValue()
    {
        return Random.Range(skillInfo.minDamage[actionLv], skillInfo.minDamage[actionLv]);
    }

    public virtual void OnIncreaseDamage(ref ActionPerformInfo info)
    {
        
    }

    public virtual void OnPerformAction()
    {

    }
}

public class Action_ : Action_Base { }
public class Action_Cut : Action_Base { }
public class Action_EnhanceCut : Action_Base { }
public class Action_Stab : Action_Base { }
public class Action_EnhanceStab : Action_Base { }
public class Action_Smashing : Action_Base { }
public class Action_EnhanceSmashing : Action_Base { }