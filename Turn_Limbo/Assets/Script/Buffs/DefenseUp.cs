using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUp : Buff_Base
{
    public override void Use(Unit target, int stack, Unit.PropertyType type)
    {
        //Debug.Log($"{target.name} {type} {stack / 10}");
        if(type == Unit.PropertyType.AllType) target.defense_Drainage -= (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.defense_Drainage -= (float)stack / 100;
    }
}
