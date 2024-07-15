using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class RoglikeManager : MonoBehaviour, IInitObserver
{
    public static RoglikeManager instance { get; private set; }

    public int Priority => 4;

    public bool isEvent;
    public int rogStageIndex;
    [SerializeField] private Controller controller;
    public SkillExplain[] skillExplains;
    public RoglikeData roglikeData;
    List<Skill> getList = new List<Skill>();
    [SerializeField] private readonly float[] sectionRandValue = { 0.1f, 0.2f, 0.3f, 0.4f};

    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        int[] tierCount = { 0, 0, 0, 0 };
        var d = DataManager.instance;
        if (d.curMode != Modes.rogLike) return;
        foreach (var canIndex in roglikeData.stageDatas[rogStageIndex].getSkillIndex)
        {
            if (d.player.selectIndex.Contains(canIndex)) continue;
            var newSkill = d.loadData.SkillList[canIndex];
            tierCount[newSkill.skillTier - 1]++;
            getList.Add(newSkill);
        }

    }

    public enum Modes
    {
        stage,
        rogLike
    }
    public void GetItemRandom()
    {
        float randValue = Random.Range(0f, 1f);
        if (randValue > 0.75f) return;

        Time.timeScale = 0;

        int[] aris = new int[getList.Count];
        float sum = 0;
        for (int i = 0; i < getList.Count; i++)
        {
            aris[i] = 4 - getList[i].skillTier;
            sum += sectionRandValue[aris[i]];
        }

        float randomAlice = Random.Range(0f, sum);
        int index = 0;
        while (randomAlice > 0)
        {
            randomAlice -= aris[index];
            if (randomAlice < 0)
            {
                break;
            }
            index++;
        }
        
    }
    public void ShowSkillGetPanel(int index)
    {

    }
    public void ShowStagePanel()
    {
        isEvent = true;
        Time.timeScale = 0;
        StartCoroutine(ShowStageAnim());
    }
    private IEnumerator ShowStageAnim()
    {
        yield return StartCoroutine(UIManager.instance.UseFadePanel());
        for (int i = 0; i < skillExplains.Length; i++)
        {
            skillExplains[i].transform.DOMoveX(-500 + (i * 500), 0.5f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.1f * (i + 1));
        }
    }
}