using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public int skillCurCount = 0;
    public int requestMaxCount = 4;
    public int requestMinCount = 1;    
    protected override void FatalDamage()
    {
        UIManager.instance.EnemyFatalDamage();
    }

    protected override void DamageLogs(int damage)
    {
        LogView.instance.curDmg[0] = damage;
    }
}
