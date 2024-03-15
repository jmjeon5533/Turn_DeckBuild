using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Skill", menuName ="Skill/AttackUp")]
public class AttackUp : Buff_Base
{
    public override void Use(Unit target, int stack, Unit.PropertyType type = Unit.PropertyType.AllType)
    {
        if(type == Unit.PropertyType.AllType) target.attack_Drainage += stack / 10;
        else if(target.curSkill.propertyType == type) target.attack_Drainage += stack / 10;
    }
}
