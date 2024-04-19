using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIBedClick : MonoBehaviour
{
    private Button btn;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => Actions());
    }
    [SerializeField] private Material optionMaterial;
    [SerializeField] private UIFadeObject fadeObject;
    private bool isShow;
    private bool isFade;

    private void Actions()
    {
        if (isFade) return;
        isFade = true;

        isShow = !isShow;
        if (isShow) fadeObject.FadeIn(0.5f, () => isFade = false);
        else fadeObject.FadeOut(0.5f, () => isFade = false);
    }
}
