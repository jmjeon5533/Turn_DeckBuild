using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogView : MonoBehaviour
{
    public static LogView instance { get; private set; }
    private RectTransform UIPanel;
    private bool isPanelMove;
    public int[] curDmg = new int[2];
    public ActionPerformInfo[] curSkills = new ActionPerformInfo[2];
    [SerializeField] RectTransform ContentPanel;
    [SerializeField] RectTransform[] Panels;
    [SerializeField] LogPanel[] BaseObjs;
    [Space(10)]
    [SerializeField] RectTransform versers;
    [SerializeField] Image baseVersers;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UIPanel = GetComponent<RectTransform>();
    }
    public void StartPanel()
    {
        if (isPanelMove) return;
        isPanelMove = true;
        StartCoroutine(PanelStart());
    }
    private IEnumerator PanelStart()
    {
        yield return UIPanel.DOLocalMove(Vector2.zero, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => isPanelMove = false).WaitForCompletion();
    }
    public void ExitPanel()
    {
        if (isPanelMove) return;
        isPanelMove = true;
        StartCoroutine(PanelExit());
    }
    private IEnumerator PanelExit()
    {
        yield return UIPanel.DOLocalMove(new Vector2(0, 1100), 0.5f).SetEase(Ease.OutQuad).OnComplete(() => isPanelMove = false).WaitForCompletion();
    }
    public void AddLogs(Sprite playerImg, Sprite enemyImg)
    {
        Sprite[] imgs = { playerImg, enemyImg };
        for (int i = 0; i < 2; i++)
        {
            var ui = Instantiate(BaseObjs[i], Panels[i]);
            ui.DamageText.text = curDmg[i].ToString();
            ui.SkillText.text = curSkills[i].skillName;
            ui.Icon.sprite = imgs[i];
        }
        ContentPanel.sizeDelta = new Vector2(0, Mathf.Clamp(Panels[0].transform.childCount * 220, 1000, 100000));
    }
}
