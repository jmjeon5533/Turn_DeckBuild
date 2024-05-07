using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelMovement : MonoBehaviour
{
    bool isStage = false;
    bool isMove = false;
    public void PanelMove()
    {
        if(isMove) return;
        isMove = true;
        isStage = !isStage;
        transform.DOMoveX((isStage ? 1 : 0) * 1920,0.5f).OnComplete(() => isMove = false);
    }
}
