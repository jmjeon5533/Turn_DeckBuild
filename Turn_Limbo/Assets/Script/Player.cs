using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    public bool coinLimit;
    public int addCoin;

    public override void TurnInit()
    {
        base.TurnInit();
        addCoin = 3;
        coinLimit = false;
    }
    public void PlusCoin(int value){
        if(coinLimit) return;
        addCoin += value;
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
