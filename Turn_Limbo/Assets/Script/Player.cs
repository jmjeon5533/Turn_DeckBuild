using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    public AnimationClip attackClip;
    protected override void Start()
    {
        base.Start();
        AnimationEvent evt = new AnimationEvent();
        evt.time = 0.4f;
        evt.functionName = "Attacking";
        attackClip.AddEvent(evt);
    }
    protected override void Update()
    {
        base.Update();
    }
}
