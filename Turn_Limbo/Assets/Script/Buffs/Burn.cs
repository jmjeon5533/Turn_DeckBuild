using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Buff_Base
{
    public override void Use(Unit target, int stack, Unit.PropertyType type)
    {
        target.Damage(target.maxHP * stack / 100,Vector3.zero);
    }
}
