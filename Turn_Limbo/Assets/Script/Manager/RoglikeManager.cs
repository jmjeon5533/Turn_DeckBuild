using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[System.Serializable]
public class RogStageData
{
    public string stageName;
    public int[] getSkillIndex;
    public Enemy[] spawnEnemyList;
    public Enemy spawnBoss;
    [Space(10)]
    public int clearGetMoney;
}
[CreateAssetMenu(fileName = "RogData", menuName = "Roglike", order = 0)]
public class RoglikeData : ScriptableObject
{
    public List<RogStageData> stageDatas = new();
}
public class RoglikeManager : MonoBehaviour, IInitObserver
{
    public static RoglikeManager instance {get; private set;}

    public int Priority => 4;

    public bool isEvent;
    public int rogStageIndex;
    [SerializeField] private Controller controller;
    public SkillExplain[] skillExplains;
    public RoglikeData roglikeData;
    List<Skill> getList = new List<Skill>();

    private void Awake()
    {
        instance = this;
    }
    public void Init()
    {
        var d = DataManager.instance;
        if(d.curMode != Modes.rogLike) return;
        foreach(var canIndex in roglikeData.stageDatas[rogStageIndex].getSkillIndex)
        {
            getList.Add(d.loadData.SkillList[canIndex]);
        }
    }
    
    public enum Modes
    {
        stage,
        rogLike
    }
    public void GetItemRandom()
    {
        float randValue = Random.Range(0f,1f);
        if(randValue > 0.75f) return;

        Time.timeScale = 0;
        
    }
    public void ShowGetSkillPanel()
    {
        isEvent = true;
        Time.timeScale = 0;
        StartCoroutine(ShowGetSkillAnim());
    }
    private IEnumerator ShowGetSkillAnim()
    {
        yield return StartCoroutine(UIManager.instance.UseFadePanel());
        for(int i = 0; i < skillExplains.Length; i++)
        {
            skillExplains[i].transform.DOMoveX(-500 + (i * 500), 0.5f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.1f * (i + 1));
        }
    }
}