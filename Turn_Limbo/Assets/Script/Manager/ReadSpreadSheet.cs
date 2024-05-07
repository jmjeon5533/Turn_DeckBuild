using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public static ReadSpreadSheet instance;
    public const string ADDRESS = "https://docs.google.com/spreadsheets/d/1ENYCDg5E6WuUwf-NZjCOpJfRufJsxQI8d7qEKh3Kf_I";
    public readonly long[] SHEET_ID = { 1705787959, 930614922 };
    public string curStageID;

    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    public List<SkillScript> skillScripts = new();
    private List<Skill> skillLists = new();

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartCoroutine(LoadData(0, ParseSkillData));
        StartCoroutine(LoadData(1, ParseTextData));
        // StartCoroutine(LoadData(0, ParseEnemyData));
    }
    public void Load(Action callBack = default)
    {
        StartCoroutine(LoadData(0, ParseSkillData, callBack));
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
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');
            KeyCode keyCode = columns[1].EnumParse<KeyCode>();
            if (!skillDatas.ContainsKey(keyCode))
                skillDatas.Add(keyCode, new List<Skill>());

            var splitExplain = columns[9].Split('&');
            string explain = string.Join("\n", splitExplain);
            var newSkill = new Skill()
            {
                skillName = columns[2],
                minDamage = int.Parse(columns[6]),
                maxDamage = int.Parse(columns[7]),
                attackCount = int.Parse(columns[8]),
                effect = skillScripts[int.Parse(columns[0])],
                keyIndex = int.Parse(columns[1]) - 1,
                actionType = columns[3].EnumParse<Unit.ActionType>(),
                propertyType = columns[4].EnumParse<Unit.PropertyType>(),
                animationName = columns[10],
                explain = explain,
                icon = Resources.Load<Sprite>($"Icon/skill{int.Parse(columns[0])}")
            };
            skillLists.Add(newSkill);
            skillDatas[keyCode].Add(newSkill);
        }
        d.skillData = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        d.SkillList = new List<Skill>(skillLists);
        Debug.Log("ReadEnd");
        //controller.inputs = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        //controller.inputLists = new List<Skill>(skillLists);
    }

    // public void ParseEnemyData(string data)
    // {
    //     string[] rows = data.Split('\n');
    //     for (int i = 1; i < rows.Length; i++)
    //     {
    //         string[] columns = rows[i].Split(',');
    //         KeyCode keyCode = columns[1].EnumParse<KeyCode>();
    //         if (!skillDatas.ContainsKey(keyCode))
    //             skillDatas.Add(keyCode, new List<Skill>());

    //         var newSkill = new Skill()
    //         {
    //             skillName = columns[3],
    //             minDamage = int.Parse(columns[6]),
    //             maxDamage = int.Parse(columns[7]),
    //             animation = Resources.Load<AnimationClip>($"Animation/{columns[10].Trim()}"),
    //             icon = Resources.Load<Sprite>($"Icon/skill{int.Parse(columns[0]) + 1}")
    //         };
    //         print(newSkill.animation == null);
    //         skillDatas[keyCode].Add(newSkill);
    //     }
    //     player_Input.inputs = new Dictionary<KeyCode, List<Skill>>(skillDatas);
    //     player_Input.InitBtn();
    // }

    public void ParseTextData(string data)
    {
        Queue<Queue<Dialogue>> dialogBox = new();
        Queue<Queue<Dialogue>> hpDialogBox = new();

        Queue<Dialogue> act = new();

        var d = DataManager.instance;
        string[] rows = data.Split('\n');
        string nowDialogueType = null;
        bool isPlayer = false;
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = Regex.Split(rows[i], ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            //Only text with the same CurStageID and stageID is imported from the sheet << Papago GO
            if (columns[0] != curStageID || columns[0] == "") continue;

            if (columns[1] != "" && act.Count != 0)
            {
                if (nowDialogueType == "StoryDialogue") dialogBox.Enqueue(new Queue<Dialogue>(act));
                else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

                act.Clear();
            }

            if (columns[1] != "") nowDialogueType = columns[1];

            var newText = new Dialogue()
            {
                name = columns[2],
                job = columns[3],
                namePos = columns[4].EnumParse<DialogueManager.NamePos>(),
                camPos = columns[5].EnumParse<DialogueManager.CamPos>(),
                text = columns[6],
                curEvent = columns[7].EnumParse<DialogueManager.CurEvent>(),
                eventValue = columns[8] != "" ? int.Parse(columns[8]) : 0,
            };

            //Debug.Log(newText.text);

            if (nowDialogueType == "HpDialogue" && columns[9] != "")
            {
                newText.hpValue = int.Parse(columns[10]);
                isPlayer = columns[9] == "Player";
            }

            act.Enqueue(newText);

            // Debug.Log($"ReadData : {id} {name} {job} {text}");
        }

        if (nowDialogueType == "StoryDialogue") dialogBox.Enqueue(new Queue<Dialogue>(act));
        else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

        d.curStageDialogBox = new Queue<Queue<Dialogue>>(dialogBox);
        d.hpDialogBox = new Queue<Queue<Dialogue>>(hpDialogBox);
        d.isPlayer = isPlayer;
    }
}