using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public float camRotZ = 0;
    public int enemyCursorIndex = 0;
    public bool isCamRotate;
    public bool isFatalEffect;
    public Camera cam;
    public Vector3 camPivot;
    public Camera bgCam;
    public Camera effectCam;
    [HideInInspector] public Vector3 camPlusPos;
    private Transform FatalTarget;

    public Image inputPanel;
    public Image[] keys;
    [SerializeField] Image[] nextKeys;
    [SerializeField] Controller controller;
    [SerializeField] Image coinGauge;

    [SerializeField] Transform dmgTextParent;
    [SerializeField] Icon baseIcon;
    [SerializeField] Image timer;
    public Image timerBG;
    public TMP_Text damageText;

    [Header("GameEnd")]
    [SerializeField] Image gameEndPanel;
    [SerializeField] TMP_Text gameEndText;
    [SerializeField] Button retry, stageSelect;
    [SerializeField] TMP_Text useTurnCountText;
    [SerializeField] TMP_Text getMoneyText;
    bool EndMove;

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
    private void Update()
    {
        if (!controller.isGame) return;

        if (controller.isAttack)
        {
            if (isFatalEffect)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3f, 0.1f);
                camPivot = Vector3.Lerp(camPivot, FatalTarget.position, 0.8f);
                camPivot = new Vector3(camPivot.x, camPivot.y, -10);
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Vector3.forward * camRotZ), 0.2f);
            }
            else
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3.5f, 0.1f);
                camPivot = Vector3.Lerp(controller.player.transform.position, controller.enemy.transform.position, 0.5f);
                camPivot = new Vector3(camPivot.x, camPivot.y, -10);
                if (isCamRotate)
                    cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Vector3.forward * camRotZ), 0.05f);
                else
                    cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Vector3.zero), 0.05f);
            }
        }
        else
        {
            timer.fillAmount = controller.gameCurTimeCount / 10;
            timer.color = Utility.ColorLerp(Color.red, Color.yellow, controller.gameCurTimeCount / 10);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,
            !controller.isTab ? 6 - Mathf.InverseLerp(10, 0, controller.gameCurTimeCount) : 3.5f, 0.1f);

            camPivot = new Vector3(0, -1.5f, -10);
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position,
        controller.isTab ? controller.enemy.transform.position + new Vector3(0, 2, 0) + camPivot : controller.movePos + camPivot + camPlusPos, 0.1f);
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
    public IEnumerator TimeSlow()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(5);
        Time.timeScale = 1;
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
    public void PlayerFatalDamage()
    {
        controller.glitch.intensity.value = 1;
        controller.color.saturation.value = -80;
        controller.color.postExposure.value = 1;
        StartCoroutine(FatalDamageTimeSlow(controller.player.transform));
    }
    public void EnemyFatalDamage()
    {
        controller.glitch.intensity.value = 1;
        controller.color.saturation.value = 25;
        controller.color.postExposure.value = 1;
        StartCoroutine(FatalDamageTimeSlow(controller.enemy.transform));
    }
    IEnumerator FatalDamageTimeSlow(Transform target)
    {
        int[] sign = {-1, 1};
        if (isFatalEffect) yield break;
        FatalTarget = target;
        camRotZ = Random.Range(5f,10f) * sign[Random.Range(0,2)];
        controller.isTimeSlowEffect = true;
        isFatalEffect = true;
        yield return new WaitForSecondsRealtime(0.75f);
        isFatalEffect = false;
        controller.isTimeSlowEffect = false;
        FatalTarget = null;
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
        EndMove = false;
        retry.onClick.AddListener(() => { if (EndMove) SceneManager.LoadScene(2); });
        stageSelect.onClick.AddListener(() => { if (EndMove) SceneManager.LoadScene(1); });

        retry.transform.DOLocalMoveY(-500, 0.2f);
        yield return stageSelect.transform.DOLocalMoveY(-500, 0.2f);
        float time = 0;
        float moneyTarget = 5000 / controller.useTurnCount;
        float countTarget = controller.useTurnCount;
        while (time < 2)
        {
            var moneyValue = Mathf.Lerp(0, moneyTarget, time);
            var countValue = Mathf.Lerp(0, countTarget, time);

            if (isWin) getMoneyText.text = $"���� �� : {Mathf.RoundToInt(moneyValue)}";
            useTurnCountText.text = $"��� �� : {Mathf.RoundToInt(countValue)}";

            time += Time.deltaTime;
            yield return null;
        }
        if (isWin) getMoneyText.text = $"���� �� : {Mathf.RoundToInt(moneyTarget)}";
        useTurnCountText.text = $"��� �� : {Mathf.RoundToInt(countTarget)}";

        if (isWin) DataManager.instance.saveData.Money += Mathf.RoundToInt(moneyTarget);
        EndMove = true;
    }
    float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
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
    
    public IEnumerator CameraShake()
    {
        Vector3 orignalCamPos = camPlusPos;

        float ranValueX = Random.Range(0, 2);
        float ranValueY = Random.Range(0, 2);
        if (ranValueX == 0) ranValueX = -1;
        if (ranValueY == 0) ranValueY = -1;

        Vector3 shakeValue = new(ranValueX / 2, ranValueY / 2);

        camPlusPos += shakeValue;
        yield return new WaitForSeconds(0.1f);
        camPlusPos = orignalCamPos + -shakeValue;
        yield return new WaitForSeconds(0.1f);
        camPlusPos = orignalCamPos;
    }
}
