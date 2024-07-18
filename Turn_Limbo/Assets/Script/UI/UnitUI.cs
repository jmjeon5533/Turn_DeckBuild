using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Text;

public class UnitUI : MonoBehaviour
{
    public RectTransform requestUIParent;
    public RectTransform requestBuffParent;
    public RectTransform statParent;
    public RectTransform buffParent;
    [SerializeField] protected GameObject status;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image hpAnimImage;
    [SerializeField] protected Image shieldImage;
    [SerializeField] protected Image shieldAnimImage;
    public TMP_Text buffText;

    [HideInInspector] public StringBuilder stringBuilder = new();
    [HideInInspector] public TMP_Text curText;

    public virtual void InitUnit()
    {

    }
    public void UIUpdate(Transform target, int hp, int maxHP, int shield, int maxShield, ref float dmgDelayCurTime, bool isLeft)
    {
        var ui = UIManager.instance;
        statParent.anchoredPosition
        = ui.cam.WorldToScreenPoint(target.transform.localPosition + (new Vector3(-2f, 0) * (isLeft ? 1 : -1)));
        buffParent.anchoredPosition
        = ui.cam.WorldToScreenPoint(target.transform.localPosition);
        hpImage.fillAmount = (float)hp / maxHP;
        if (dmgDelayCurTime <= 0)
        {
            hpAnimImage.fillAmount = Mathf.MoveTowards(hpAnimImage.fillAmount, hpImage.fillAmount, Time.deltaTime);
            shieldAnimImage.fillAmount = Mathf.MoveTowards(shieldAnimImage.fillAmount, shieldImage.fillAmount, Time.deltaTime);
        }
        else dmgDelayCurTime -= Time.deltaTime;
        shieldImage.fillAmount = (float)shield / maxShield;

        requestUIParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
        requestBuffParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
    }
    public void BuffText(string name)
    {
        TMP_Text text;

        if(curText == null){
            text = Instantiate(buffText, buffParent);
            curText = text;
            text.rectTransform.localPosition += new Vector3(0, -300);
        }
        else text = curText;

        stringBuilder.Append("\n" + name);

        text.text = stringBuilder.ToString();

        text.rectTransform.DOAnchorPosY(text.rectTransform.anchoredPosition.y + 150, 0.8f).OnComplete(() => Destroy(text.gameObject));
    }
    public void HideUI(bool isOn)
    {
        requestUIParent.gameObject.SetActive(isOn);
        requestBuffParent.gameObject.SetActive(isOn);
        status.SetActive(isOn);
    }
}
