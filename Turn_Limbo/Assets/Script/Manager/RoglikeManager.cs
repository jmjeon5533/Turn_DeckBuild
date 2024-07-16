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
    public GameObject[] skillExplainPivots;
    public StageExplain[] stageExplains;
    public RoglikeData roglikeData;
    [SerializeField] private List<Skill> getList = new List<Skill>();
    [SerializeField] TMP_Text[] btnTexts;
    [SerializeField] private Skill curSkill;
    [SerializeField] private readonly float[] sectionRandValue = { 0.1f, 0.2f, 0.3f, 0.4f };

    [SerializeField] private Text eventExplainText;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
            Debug.Log("null: " + skillExplainPivots);
    }
    public void Init()
    {
        skillExplainPivots[0].transform.localPosition = new Vector3(-1500, -105);
        skillExplainPivots[1].transform.localPosition = new Vector3(1000, -105);
        eventExplainText.transform.localPosition = new Vector3(0, 650);
        for (int i = 0; i < stageExplains.Length; i++)
        {
            stageExplains[i].transform.DOMoveY(-800, 0);
        }
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
        if (controller.spawnCount >= roglikeData.stageDatas[rogStageIndex].spawnEnemyList.Length)
        {
            print("break");
            return;
        }
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

        skillExplainPivots[0].transform.DOLocalMoveX(-428, 0.5f).SetUpdate(true);
        skillExplainPivots[1].transform.DOLocalMoveX(125, 0.5f).SetUpdate(true);

        StartCoroutine(UIManager.instance.OnFadePanel());
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
        StartCoroutine(UIManager.instance.OffFadePanel());
        eventExplainText.transform.DOLocalMoveY(650, 0.5f).SetUpdate(true);
        skillExplainPivots[0].transform.localPosition = new Vector3(-1500, -105);
        skillExplainPivots[1].transform.localPosition = new Vector3(1000, -105f);
    }
    public void ShowStagePanel()
    {
        isEvent = true;
        Time.timeScale = 0;
        StartCoroutine(ShowStageAnim(true));
    }
    private IEnumerator ShowStageAnim(bool isOn)
    {
        eventExplainText.transform.DOLocalMoveY(450, 0.5f).SetUpdate(true);
        StartCoroutine(UIManager.instance.OnFadePanel());
        var targetY = isOn ? 112 : -800;
        List<RogStageData> stages = new List<RogStageData>();

        foreach (var stage in roglikeData.stageDatas)
        {
            stages.Add(stage);
        }
        for (int i = 0; i < stageExplains.Length; i++)
        {
            int randIndex = Random.Range(0, stages.Count);
            var curStage = stages[randIndex];
            var index = randIndex;
            stageExplains[i].SetExplain(stages[randIndex], controller);
            stageExplains[i].btn.onClick.RemoveAllListeners();
            stageExplains[i].btn.onClick.AddListener(() =>
            {
                rogStageIndex = index;
                StageMove(curStage);
            });
            stages.RemoveAt(randIndex);
            var speed = 0.5f + (i * 0.1f);
            MoveStagePanel(i, targetY, speed);
            yield return new WaitForSecondsRealtime(0.1f + (i * 0.1f));
            print(i);
        }
    }
    private void MoveStagePanel(int index, float targetY, float speed)
    {
        stageExplains[index].transform.DOLocalMoveY(targetY, speed).SetEase(Ease.OutQuad).SetUpdate(true);
    }
    public void StageMove(RogStageData rogStageData)
    {
        for (int i = 0; i < stageExplains.Length; i++)
        {
            var speed = 0.5f + (i * 0.1f);
            MoveStagePanel(i, -800, speed);
        }
        StartCoroutine(fadeBg(rogStageData));
        eventExplainText.transform.DOLocalMoveY(650, 0.5f).SetUpdate(true);
        StartCoroutine(UIManager.instance.OffFadePanel());
    }
    IEnumerator fadeBg(RogStageData rogStageData)
    {
        yield return controller.bg.DOColor(Color.black, 0.3f).SetUpdate(true).WaitForCompletion();
        controller.bg.sprite = rogStageData.Bg;
        yield return controller.bg.DOColor(Color.white, 0.3f).SetUpdate(true).WaitForCompletion();
        controller.spawnCount = 0;
        isEvent = false;
    }
}