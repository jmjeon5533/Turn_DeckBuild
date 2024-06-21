using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private Vector2 adjust;
    [SerializeField] private Unit target;
    [SerializeField] private RectTransform skillSpriteUIParent;
    [SerializeField] private RectTransform skillSpriteBuffParent;
    [SerializeField] private RectTransform statParent;
    [SerializeField] private GameObject status;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpAnimImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image shieldAnimImage;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        transform.position = (Vector2)cam.WorldToScreenPoint(target.transform.position) + adjust;

        hpImage.fillAmount = (float)target.Hp / target.MaxHp;
        hpAnimImage.fillAmount = Mathf.Lerp(hpAnimImage.fillAmount, hpImage.fillAmount, Time.deltaTime);

        shieldImage.fillAmount = (float)target.Shield / target.MaxShield;
        shieldAnimImage.fillAmount = Mathf.Lerp(shieldAnimImage.fillAmount, shieldImage.fillAmount, Time.deltaTime);
    }

    // public void HideUI(bool isOn)
    // {
    //     skillSpriteUIParent.gameObject.SetActive(isOn);
    //     skillSpriteBuffParent.gameObject.SetActive(isOn);
    //     status.SetActive(isOn);
    // }
}
