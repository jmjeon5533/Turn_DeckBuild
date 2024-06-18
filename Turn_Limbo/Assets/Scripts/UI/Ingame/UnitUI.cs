using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public enum Target
    {
        Player,
        Enemy
    }

    //singleton
    private UIManager uiManager => UIManager.instance;

    [SerializeField] private Target target;
    [SerializeField] private RectTransform skillSpriteUIParent;
    [SerializeField] private RectTransform skillSpriteBuffParent;
    [SerializeField] private RectTransform statParent;
    [SerializeField] private GameObject status;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpAnimImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image shieldAnimImage;
    
    private Unit unit;

    private void Start()
    {
        
    }

    public virtual void InitUnit()
    {
        hpAnimImage = statParent.GetChild(1).GetComponent<Image>();
        hpImage = hpAnimImage.transform.GetChild(0).GetComponent<Image>();

        shieldAnimImage = statParent.GetChild(3).GetComponent<Image>();
        shieldImage = shieldAnimImage.transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        statParent.anchoredPosition =
            uiManager.cam.WorldToScreenPoint(unit.transform.localPosition + (new Vector3(-2f, 0) * (unit.isLeft ? 1 : -1)));

        hpImage.fillAmount = (float)unit.hp / unit.maxHP;
        if (unit.dmgDelayCurTime <= 0)
        {
            hpAnimImage.fillAmount = Mathf.MoveTowards(hpAnimImage.fillAmount, hpImage.fillAmount, Time.deltaTime);
            shieldAnimImage.fillAmount = Mathf.MoveTowards(shieldAnimImage.fillAmount, shieldImage.fillAmount, Time.deltaTime);
        }
        else
        {
            unit.dmgDelayCurTime -= Time.deltaTime;
        }
        shieldImage.fillAmount = (float)unit.shield / unit.maxShield;

        skillSpriteUIParent.localScale = Vector3.one * (1 + (5 - uiManager.cam.orthographicSize) * 0.3f);
        skillSpriteBuffParent.localScale = Vector3.one * (1 + (5 - uiManager.cam.orthographicSize) * 0.3f);
    }

    // public void HideUI(bool isOn)
    // {
    //     skillSpriteUIParent.gameObject.SetActive(isOn);
    //     skillSpriteBuffParent.gameObject.SetActive(isOn);
    //     status.SetActive(isOn);
    // }
}
