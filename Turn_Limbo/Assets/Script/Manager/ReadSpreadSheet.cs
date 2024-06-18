using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public static ReadSpreadSheet instance;
    public const string ADDRESS = "https://docs.google.com/spreadsheets/d/1ENYCDg5E6WuUwf-NZjCOpJfRufJsxQI8d7qEKh3Kf_I";
    public readonly long[] SHEET_ID = { 1705787959, 930614922, 520277150, 232901544 };

    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    private List<Skill> skillLists = new();

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Load(Action callBack = default)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(LoadData(0, ParseSkillData, callBack));
            StartCoroutine(LoadData(1, ParseTextData));
            StartCoroutine(LoadData(2, ParseEnemyData));
            StartCoroutine(LoadData(3, PasreBuffData));
        }
    }
    private IEnumerator LoadData(int pageIndex, Action<string> dataAction, Action callBack = default)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetCSVAddress(SHEET_ID[pageIndex]));
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        dataAction?.Invoke(data);
        callBack?.Invoke();
    }

    public static string GetCSVAddress(long sheetID)
    {
        return $"{ADDRESS}/export?format=csv&gid={sheetID}";
    }

    public void ParseSkillData(string data)
    {
        Debug.Log("Read");

        var d = DataManager.instance;
        string[] rows = data.Split('\n');

        skillLists.Clear();
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');
            KeyCode keyCode = columns[1].EnumParse<KeyCode>();
            if (!skillDatas.ContainsKey(keyCode))
                skillDatas.Add(keyCode, new List<Skill>());

            var splitExplain = columns[8].Split('&');
            string explain = string.Join("\n", splitExplain);
            var newSkill = new Skill();
            newSkill.index = i;
            newSkill.skillName = columns[2];
            newSkill.cost = new int[3];
            newSkill.minDamage = new int[3];
            newSkill.maxDamage = new int[3];
            newSkill.attackCount = int.Parse(columns[5]);
            newSkill.keyIndex = int.Parse(columns[1]) - 1;
            newSkill.actionType = columns[3].EnumParse<Unit.ActionType>();
            newSkill.propertyType = columns[4].EnumParse<PropertyType>();
            //newSkill.animationName = columns[8];
            newSkill.animationName = columns[4];
            newSkill.effect_desc = explain;
            newSkill.skill_desc = columns[7];
            newSkill.icon = Resources.Load<Sprite>($"Icon/skill{int.Parse(columns[0])}");

            string className = "Skill_" + columns[6];
            newSkill.effect = Activator.CreateInstance(Type.GetType(className)) as Skill_Base;

            for (int j = 0; j < 3; j++)
            {
                newSkill.cost[j] = int.Parse(columns[10 + (j * 3)]);
                newSkill.minDamage[j] = int.Parse(columns[11 + (j * 3)]);
                newSkill.maxDamage[j] = int.Parse(columns[12 + (j * 3)]);
            }
            skillLists.Add(newSkill);
            skillDatas[keyCode].Add(newSkill);
        }
        d.loadData.skillData = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        d.loadData.SkillList.Clear();
        d.loadData.SkillList = new List<Skill>(skillLists);
        Debug.Log("ReadEnd");
        //controller.inputs = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        //controller.inputLists = new List<Skill>(skillLists);
    }

    void PasreBuffData(string data)
    {
        Dictionary<string, Buff_Base> buff = new();
        Dictionary<string, Buff_Base> debuff = new();

        var d = DataManager.instance;
        string[] rows = data.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            string className = "Buff_" + columns[2];
            var temp = Activator.CreateInstance(Type.GetType(className)) as Buff_Base;
            temp.timing = columns[3].EnumParse<BuffTiming>();
            temp.buffIcon = Resources.Load<Sprite>($"BuffIcon/{columns[2]}");
            //Debug.Log($"Icon/BuffIcon/{columns[2]}");

            if (columns[4] == "buff" && columns[4] != "") buff.Add(columns[2], temp);
            else debuff.Add(columns[2], temp);
        }

        d.loadData.buffList = buff;
        d.loadData.debuffList = debuff;
    }

    void ParseTextData(string data)
    {
        var d = DataManager.instance;

        Queue<Dialogue> dialogBox = new();
        Queue<Queue<Dialogue>> hpDialogBox = new();

        Queue<Dialogue> act = new();

        bool isPlayer = false;
        Debug.Log("ReadDialogue");

        string[] rows = data.Split('\n');
        string nowDialogueType = null;
        int stageIndex = 0;

        d.loadData.stageDialogBox.Clear();
        d.loadData.hpDialogBox.Clear();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            if (columns[1] != "") nowDialogueType = columns[1];

            var splitExplain = columns[6].Split('&');
            string explain = string.Join(",", splitExplain);
            var newText = new Dialogue()
            {
                name = columns[2],
                job = columns[3],
                namePos = columns[4].EnumParse<DialogueManager.NamePos>(),
                camPos = columns[5].EnumParse<DialogueManager.CamPos>(),
                text = explain,
                curEvent = columns[7].EnumParse<DialogueManager.CurEvent>(),
                eventValue = columns[8] != "" ? columns[8] : "",
            };

            //Debug.Log(newText.text);

            if (nowDialogueType == "HpDialogue" && columns[9] != "")
            {
                newText.hpValue = int.Parse(columns[10]);
                isPlayer = columns[9] == "Player";
            }

            act.Enqueue(newText);

            if (i == rows.Length - 1)
            {
                if (nowDialogueType == "StoryDialogue") dialogBox = act;
                else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

                d.loadData.stageDialogBox.Add(stageIndex, dialogBox);
                d.loadData.hpDialogBox.Add(stageIndex, hpDialogBox);

                continue;
            }

            string[] nextColumns = rows[i + 1].Split(',');

            if (nextColumns[1] != "" && act.Count != 0)
            {
                if (nowDialogueType == "StoryDialogue") dialogBox = new Queue<Dialogue>(act);
                else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

                act.Clear();
            }
            if (stageIndex != int.Parse(nextColumns[0]) && nextColumns[0] != "")
            {
                d.loadData.stageDialogBox.Add(stageIndex, new Queue<Dialogue>(dialogBox));
                d.loadData.hpDialogBox.Add(stageIndex, new Queue<Queue<Dialogue>>(hpDialogBox));

                stageIndex = int.Parse(nextColumns[0]);
            }
        }

        d.readEnd = true;
        d.hpUnitIsPlayer = isPlayer;
    }

    public void ParseEnemyData(string data)
    {
        var d = DataManager.instance;
        string[] row = data.Split("\n");
        d.loadData.enemyData.Clear();
        for (int i = 1; i < row.Length; i++)
        {
            string[] column = row[i].Split(",");
            UnitData newEnemy = new();
            newEnemy.index = int.Parse(column[0]);
            newEnemy.name = column[1];
            newEnemy.hp = int.Parse(column[2]);
            newEnemy.shield = int.Parse(column[3]);
            newEnemy.atk = int.Parse(column[4]);
            newEnemy.minCount = int.Parse(column[5]);
            newEnemy.maxCount = int.Parse(column[6]);

            d.loadData.enemyData.Add(newEnemy);
        }
    }
}