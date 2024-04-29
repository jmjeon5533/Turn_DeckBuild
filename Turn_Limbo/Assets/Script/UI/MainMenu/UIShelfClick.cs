using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIShelfClick : MonoBehaviour
{
    private Button btn;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => Actions());
    }
    [SerializeField] private UIFadeObject fadeObject;

    private void Actions()
    {
        if (fadeObject.isFade) return;
        fadeObject.isFade = true;

        fadeObject.isShow = !fadeObject.isShow;
        if (fadeObject.isShow) fadeObject.FadeIn(0.5f, () => { fadeObject.isFade = false; });
        else fadeObject.FadeOut(0.5f, () => fadeObject.isFade = false);
    }
}
