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
    protected readonly ActionData info;
    protected readonly Unit caster;
    protected readonly int lv;
    protected Unit target;

    /// <summary>
    /// Do not use this constructor
    /// </summary>
    public Action_Base() { }
    public Action_Base(ActionData info, Unit caster, int lv)
    {
        this.info = info;
        this.caster = caster;
        this.lv = lv;
    }

    public void SetTarget(Unit target)
    {
        this.target = target;
    }

    public virtual void OnActionStart()
    {

    }

    public virtual int GetActionValue()
    {
        return Random.Range(info.minDamage[lv], info.minDamage[lv]);
    }

    public virtual void OnIncreaseDamage(ref ActionPerformData info)
    {
        
    }

    public virtual void OnPerformAction()
    {

    }
}

public class Action_Cut : Action_Base { }
public class Action_EnhanceCut : Action_Base { }
public class Action_Stab : Action_Base { }
public class Action_EnhanceStab : Action_Base { }
public class Action_Smashing : Action_Base { }
public class Action_EnhanceSmashing : Action_Base { }