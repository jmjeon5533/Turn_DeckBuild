using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System;

public class UIFadeObject : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    public void FadeIn(float time, Action onComplete = null)
    {
        gameObject.SetActive(true);
        targetMaterial
            .DOColor(Color.white, time)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(float time, Action onComplete = null)
    {
        targetMaterial
            .DOColor(new Color(1, 1, 1, 0), time)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }
}