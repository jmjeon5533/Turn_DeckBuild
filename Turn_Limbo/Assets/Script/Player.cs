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
    protected override void Update()
    {
        base.Update();
    }
    protected override void FatalDamage()
    {
        UIManager.instance.PlayerFatalDamage();
    }
    protected override void DamageLogs(int damage)
    {
        LogView.instance.curDmg[1] = damage;
    }
}
