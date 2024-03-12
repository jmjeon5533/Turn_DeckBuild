using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    protected override void Update()
    {
        base.Update();
    }
    protected override void FatalDamage()
    {
        UIManager.instance.FatalDamage();
    }
}
