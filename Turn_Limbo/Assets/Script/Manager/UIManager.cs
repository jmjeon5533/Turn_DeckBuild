using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class UnitUI
{
    public RectTransform requestParent,
    requestBuffParent,
    statParent;
}
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public float camRotZ = 0;
    public int enemyCursorIndex = 0;
    public bool isCamRotate;
    public Camera cam;
    public Camera bgCam;
    public Camera effectCam;

    public Image inputPanel;
    public Image[] keys;
    [SerializeField] Image[] nextKeys;
    [SerializeField] Controller controller;
    [SerializeField] Image coinGauge;

    [SerializeField] Transform dmgTextParent;
    [SerializeField] Icon baseIcon;
    [SerializeField] Image timer;
    [SerializeField] Image timerBG;
    public TMP_Text damageText;

    [Header("GameEnd")]
    [SerializeField] Image gameEndPanel;
    [SerializeField] TMP_Text gameEndText;
    [SerializeField] Button retry, stageSelect;

    [Header("ExplainText")]
    [SerializeField] Image skillExplainPanel;
    [SerializeField] TMP_Text skillExplainText;
    [Space(5)]
    [SerializeField] Image enemySkillExplainPanel;
    [SerializeField] TMP_Text enemySkillExplainText;

    [Header("Unit")]
    [SerializeField] UnitUI[] unitUI;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SelectEnemyImage(false);
    }
    public void InitUnitParent(Unit unit, int index)
    {
        unit.requestBuffParent = unitUI[index].requestBuffParent;
        unit.requestUIParent = unitUI[index].requestParent;
        unit.statParent = unitUI[index].statParent;
    }
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
            timer.fillAmount = controller.gameCurTimeCount / 10;
            timer.color = Utility.ColorLerp(Color.red, Color.yellow, controller.gameCurTimeCount / 10);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,
            !controller.isTab ? 6 - Mathf.InverseLerp(10, 0, controller.gameCurTimeCount) : 3.5f, 0.1f);

            camPos = new Vector3(0, -1.5f, -10);
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position,
        controller.isTab ? controller.enemy.transform.position + new Vector3(0, 2, 0) + camPos : controller.movePos + camPos, 0.1f);
        if (!controller.isAttack)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) SelectEnemyImage(true);
            if (Input.GetKeyUp(KeyCode.Tab)) SelectEnemyImage(false);
        }
        if (controller.isTab)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                enemyCursorIndex--;
                SelectEnemyImage(true);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                enemyCursorIndex++;
                SelectEnemyImage(true);
            }
        }

        bgCam.orthographicSize = cam.orthographicSize;
        effectCam.orthographicSize = cam.orthographicSize;
    }
    public void SelectEnemyImage(bool isActive)
    {
        var request = controller.enemy.attackRequest;
        enemySkillExplainPanel.gameObject.SetActive(isActive);
        for (int i = 0; i < request.Count; i++)
        {
            request[i].insertImage.selected.enabled = false;
        }
        if (!isActive) return;
        enemyCursorIndex = Mathf.Clamp(enemyCursorIndex, 0, controller.enemy.attackRequest.Count - 1);
        request[enemyCursorIndex].insertImage.selected.enabled = isActive;
        enemySkillExplainText.text = request[enemyCursorIndex].explain;
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
        img.iconImage.sprite = sprite;
        img.selected.enabled = false;
        return img.iconImage;
    }
    public Icon AddIcon(Sprite sprite, Transform parent)
    {
        var img = Instantiate(baseIcon, parent);
        img.iconImage.sprite = sprite;
        img.selected.enabled = false;
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
        yield return new WaitForSeconds(0.2f);
        retry.onClick.AddListener(() => SceneManager.LoadScene(2));
        stageSelect.onClick.AddListener(() => SceneManager.LoadScene(1));

        retry.transform.DOLocalMoveY(-500,0.2f);
        stageSelect.transform.DOLocalMoveY(-500,0.2f);
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
}
