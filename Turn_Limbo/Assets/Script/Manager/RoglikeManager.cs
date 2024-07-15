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
    public SkillExplain skillExplain;
    public GameObject skillExplainObject;
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

        isEvent = true;
        Time.timeScale = 0;

        int[] flipList = new int[getList.Count];
        float sum = 0;
        for (int i = 0; i < getList.Count; i++)
        {
            flipList[i] = 4 - getList[i].skillTier;
            sum += sectionRandValue[flipList[i]];
        }

        float rand = Random.Range(0f, sum);
        int index = 0;
        while (rand > 0)
        {
            rand -= flipList[index];
            if (rand < 0)
            {
                break;
            }
            index++;
        }
        ShowSkillGetPanel(index);
    }
    public void ShowSkillGetPanel(int index)
    {
        skillExplainObject.SetActive(true);
        var skill = DataManager.instance.loadData.SkillList[index];
        skillExplain.ExplainSet(skill,skill.level);
    }
    // public void ShowStagePanel()
    // {
    //     isEvent = true;
    //     Time.timeScale = 0;
    //     StartCoroutine(ShowStageAnim());
    // }
    // private IEnumerator ShowStageAnim()
    // {
    //     yield return StartCoroutine(UIManager.instance.UseFadePanel());
    //     for (int i = 0; i < skillExplains.Length; i++)
    //     {
    //         skillExplains[i].transform.DOMoveX(-500 + (i * 500), 0.5f).SetEase(Ease.OutQuad);
    //         yield return new WaitForSeconds(0.1f * (i + 1));
    //     }
    // }
}