using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RoglikeManager : MonoBehaviour, IInitObserver
{
    public static RoglikeManager instance { get; private set; }

    public int Priority => 4;

    public bool isEvent;
    public int rogStageIndex = 0;
    [SerializeField] private Controller controller;
    public SkillExplain skillExplain;
    public StageExplain[] stageExplains;
    public GameObject skillExplainObject;
    public RoglikeData roglikeData;
    [SerializeField] private List<Skill> getList = new List<Skill>();
    [SerializeField] TMP_Text[] btnTexts;
    [SerializeField] private Skill curSkill;
    [SerializeField] private readonly float[] sectionRandValue = { 0.1f, 0.2f, 0.3f, 0.4f };

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
            Debug.Log("null: " + skillExplainObject);
    }
    public void Init()
    {
        print("Rog");
        skillExplainObject.SetActive(false);
        RebaseCanGetSkill();
    }
    public void RebaseCanGetSkill()
    {
        int[] tierCount = { 0, 0, 0, 0 };
        var d = DataManager.instance;
        if (d.curMode != Modes.rogLike) return;
        getList.Clear();
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
        print($"{rand} / {sum}");
        int index = 0;
        while (rand > 0)
        {
            rand -= sectionRandValue[flipList[index]];
            if (rand < 0)
            {
                break;
            }
            index++;
        }
        ShowSkillGetPanel(getList[index], index);
    }
    public void ShowSkillGetPanel(Skill skill, int index)
    {
        KeyCode[] IndexToKey = { KeyCode.Q, KeyCode.W, KeyCode.E };

        skillExplainObject.SetActive(true);
        curSkill = skill;
        getList.RemoveAt(index);
        skillExplain.ExplainSet(curSkill, curSkill.level);
        btnTexts[0].text = $"<size=100>{IndexToKey[curSkill.keyIndex]}</size> À§Ä¡¿¡ ÇÒ´ç";
        btnTexts[1].text = $"<size=100>{curSkill.sale}¿ø</size> È¹µæ";
    }
    public void GetSkill()
    {
        var d = DataManager.instance;
        controller.inputLists.Add(d.loadData.SkillList[curSkill.index - 1]);
        controller.inputs[curSkill.keyIndex].Add(curSkill);
        HoldSkills newSkill = new HoldSkills()
        {
            holdIndex = curSkill.index - 1,
            level = 0
        };
        controller.player.skillInfo.holdSkills.Add(curSkill.index - 1, newSkill);
        EndEvent();
    }
    public void SaleSkill()
    {
        DataManager.instance.saveData.money += curSkill.sale;
        EndEvent();
    }
    public void EndEvent()
    {
        isEvent = false;
        skillExplainObject.SetActive(false);
    }
    public void ShowStagePanel()
    {
        isEvent = true;
        Time.timeScale = 0;
        StartCoroutine(ShowStageAnim(true));
    }
    private IEnumerator ShowStageAnim(bool isOn)
    {
        StartCoroutine(UIManager.instance.OnFadePanel());
        var targetY = isOn ? 275 : -800;
        for (int i = 0; i < stageExplains.Length; i++)
        {
            var speed = 0.5f + (i * 0.1f);
            stageExplains[i].transform.DOLocalMoveY(targetY, speed).SetEase(Ease.OutQuad).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.1f + (i * 0.1f));
            print(i);
        }
    }
}