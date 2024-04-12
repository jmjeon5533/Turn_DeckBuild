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
    public int playerDmg, enemyDmg;
    public RequestSkill playerSkill, enemySkill;
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
        var player = Instantiate(BaseObjs[0], Panels[0]);
        player.DamageText.text = playerDmg.ToString();
        player.SkillText.text = playerSkill.skillName;
        player.Icon.sprite = playerImg;
        var enemy = Instantiate(BaseObjs[1], Panels[1]);
        enemy.DamageText.text = enemyDmg.ToString();
        enemy.Icon.sprite = enemyImg;
        enemy.SkillText.text = enemySkill.skillName;
    }
}
