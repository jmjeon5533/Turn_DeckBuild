using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AttackView : MonoBehaviour
{
    [SerializeField] RectTransform upBox;
    [SerializeField] RectTransform downBox;

    [SerializeField] RectTransform chainBox;
    [SerializeField] Text text;

    public void Reset()
    {
        if (transform.rotation != Quaternion.identity) transform.DORotate(Vector3.zero, 0.15f);
    }

    public void OnOff(bool isOnOff){
        if(!isOnOff) Reset();
        upBox.DOAnchorPosY(isOnOff ? 50 : 100, 0.3f).SetUpdate(true);
        downBox.DOAnchorPosY(isOnOff ? -50 : -100, 0.3f).SetUpdate(true);
    }

    public IEnumerator ChainAttack(bool isPlayer, string chainText){
        chainBox.localPosition = new Vector3(isPlayer ? 1500 : -1500, 400);
        chainBox.DOAnchorPosX(isPlayer ? 560 : -560, 0.1f);
        StartCoroutine(TypingText(chainText));
        yield return new WaitForSeconds(0.5f);
        chainBox.DOAnchorPosX(isPlayer ? 1500 : -1500, 0.1f);
    }

    IEnumerator TypingText(string chainText)
    {
        text.text = null;
        for (int i = 0; i < chainText.Length; i++)
        {
            text.text += chainText[i];
            yield return 1f / chainText.Length;
        }
    }
}
