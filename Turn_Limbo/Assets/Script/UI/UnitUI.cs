using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public RectTransform requestUIParent;
    public RectTransform requestBuffParent;
    public RectTransform statParent;
    [SerializeField] private Image[] Bg;
    [SerializeField] private GameObject status;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpAnimImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image shieldAnimImage;

    public virtual void InitUnit()
    {
        
    }
    public void UIUpdate(Transform target,int hp, int maxHP, int shield, int maxShield, ref float dmgDelayCurTime, bool isLeft)
    {
        bool isShield = shield > 0;
        Bg[0].enabled = !isShield;
        Bg[1].enabled = isShield;

        var ui = UIManager.instance;
        statParent.anchoredPosition
        = ui.cam.WorldToScreenPoint(target.transform.localPosition + (new Vector3(-2f, 0) * (isLeft ? 1 : -1)));
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
    public void HideUI(bool isOn)
    {
        requestUIParent.gameObject.SetActive(isOn);
        requestBuffParent.gameObject.SetActive(isOn);
        status.SetActive(isOn);
    }
}
