using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public int skillCurCount = 0;
    public int requestMaxCount = 4;
    public int requestMinCount = 1;
    public override void InitUnit()
    {
        UIManager.instance.InitUnitParent(this,1);
        base.InitUnit();
    }
    protected override void FatalDamage()
    {
        
    }
}
