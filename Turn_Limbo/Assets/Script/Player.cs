using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    public int addCoin;

    public override void TurnInit()
    {
        base.TurnInit();
        addCoin = 3;
    }
    public override void InitUnit()
    {
        UIManager.instance.InitUnitParent(this,0);
        base.InitUnit();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void FatalDamage()
    {
        UIManager.instance.FatalDamage();
    }
    protected override void DamageLogs(int damage)
    {
        LogView.instance.curDmg[1] = damage;
    }
}
