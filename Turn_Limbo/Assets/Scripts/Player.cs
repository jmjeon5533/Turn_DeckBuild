using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    public int addCoin;

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        addCoin = 3;
    }
}
