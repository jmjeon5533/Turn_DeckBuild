using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseDown : Buff_Base
{
    public override void Use(Unit target, int stack, Unit.PropertyType type)
    {
        if(type == Unit.PropertyType.AllType) target.defense_Drainage += (float)stack / 100;
        else if(target.curSkill.propertyType == type) target.defense_Drainage += (float)stack / 100;
    }
}
