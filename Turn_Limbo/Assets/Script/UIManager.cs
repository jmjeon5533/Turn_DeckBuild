using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Serialization;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    public float camRotZ = 0;
    public bool isCamRotate;
    public Camera cam;
    public Camera bgCam;
    public Camera effectCam;
    [HideInInspector] public Vector3 camPlusPos;

    public Image inputPanel;
    public Image[] keys;
    [SerializeField] Image[] nextKeys;
    [SerializeField] Controller controller;
    [SerializeField] Image coinGauge;

    [SerializeField] Transform dmgTextParent;
    [SerializeField] Image baseIcon;
    [SerializeField] Image timer;
    public Image timerBG;
    public TMP_Text damageText;

    [Header("GameEnd")]
    [SerializeField] Image gameEndPanel;
    [SerializeField] TMP_Text gameEndText;

    [Header("DamageText")]
    [SerializeField] Image skillExplainPanel;
    [SerializeField] TMP_Text skillExplainText;


    private void Update()
    {
        if (!controller.isGame) return;
        if (isCamRotate)
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Vector3.forward * camRotZ), 0.05f);
        Vector3 camPos;
        if (controller.isAttack)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3.5f, 0.1f);
            camPos = new Vector3(0, 0, -10);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 6 - Mathf.InverseLerp(10, 0, controller.gameCurTimeCount), 0.1f);
            timer.fillAmount = controller.gameCurTimeCount / 10;
            timer.color = Utility.ColorLerp(Color.red, Color.yellow, controller.gameCurTimeCount / 10);
            camPos = new Vector3(0, -1.5f, -10);
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position, controller.movePos + camPos + camPlusPos, 0.1f);
        bgCam.orthographicSize = cam.orthographicSize;
        effectCam.orthographicSize = cam.orthographicSize;
        if(Input.GetKeyDown(KeyCode.Y)){StartCoroutine(CameraShake());}
    }
    public void FatalDamage()
    {
        controller.glitch.intensity.value = 1;
        controller.color.saturation.value = -80;
        controller.color.postExposure.value = 1;
    }
    public Image AddImage(Sprite sprite, Transform parent)
    {
        var img = Instantiate(baseIcon, parent);
        img.sprite = sprite;
        return img;
    }
    public void ChangeCoinSkillImg()
    {
        bool isActiveBtn = controller.useAbleCoin > 0;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].color = isActiveBtn ? Color.white : Color.grey;
        }
        coinGauge.fillAmount = (float)controller.useAbleCoin / 10;
    }
    public void ActiveBtn(bool isActive)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].enabled = isActive;
            nextKeys[i].enabled = isActive;
        }
        coinGauge.enabled = isActive;
        if (!isActive) timerBG.rectTransform.DOAnchorPosY(100, 0.5f).SetEase(Ease.OutCubic);
        else timerBG.rectTransform.anchoredPosition = new Vector2(0, -75f);
    }
    public void NextImage(int index, Sprite sprite, Sprite nextSprite)
    {
        keys[index].sprite = sprite;
        nextKeys[index].sprite = nextSprite;
    }
    public void SetExplain(bool isActive, Skill skill = null, Vector3 pos = default)
    {
        skillExplainPanel.gameObject.SetActive(isActive);
        skillExplainPanel.rectTransform.anchoredPosition = pos + new Vector3(350f, 300);
        if (skill != null) skillExplainText.text = skill.explain;
    }
    public void SetGameEndUI(bool isWin)
    {
        StartCoroutine(SetGameEnd(isWin));
    }
    IEnumerator SetGameEnd(bool isWin)
    {
        string text = isWin ? "Victory" : "Defeat";
        yield return gameEndPanel.DOColor(new Color(0, 0, 0, 0.5f), 0.5f).SetUpdate(true).WaitForCompletion();
        gameEndText.text = text;
    }
    public void DamageText(int damage, Vector3 pos)
    {
        var text = Instantiate(damageText, dmgTextParent);
        text.transform.localScale = Vector3.one * Mathf.Clamp(0.5f + (damage * 0.03f), 0.5f, 3f);
        text.text = damage.ToString();

        text.rectTransform.anchoredPosition = cam.WorldToScreenPoint(pos + (Vector3)Random.insideUnitCircle * 1.5f);

        text.transform.DOScale(0, 0.8f + (damage * 0.02f));
        text.DOColor(Color.clear, 0.8f + (damage * 0.02f)).OnComplete(() => Destroy(text.gameObject));
    }
    public IEnumerator CameraShake(){
        Vector3 orignalCamPos = camPlusPos;

        float ranValueX = Random.Range(0, 2);
        float ranValueY = Random.Range(0, 2);
        if(ranValueX == 0) ranValueX = -1;
        if(ranValueY == 0) ranValueY = -1;

        Vector3 shakeValue = new(ranValueX / 2, ranValueY / 2);

        camPlusPos += shakeValue;
        yield return new WaitForSeconds(0.1f);
        camPlusPos = orignalCamPos + -shakeValue;
        yield return new WaitForSeconds(0.1f);
        camPlusPos = orignalCamPos;
    }
}
