using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIOptionClick : MonoBehaviour, IInitObserver
{
    [SerializeField] private Button btn;
    public int Priority => 2;

    public void Init()
    {
        //btn.onClick.AddListener(() => Actions());
    }
    [SerializeField] private UIFadeObject fadeObject;


    private void Actions()
    {
        if (fadeObject.isFade) return;
        fadeObject.isFade = true;

        fadeObject.isShow = !fadeObject.isShow;
        if (fadeObject.isShow) fadeObject.FadeIn(0.5f, () => fadeObject.isFade = false);
        else fadeObject.FadeOut(0.5f, () => fadeObject.isFade = false);
    }
}
