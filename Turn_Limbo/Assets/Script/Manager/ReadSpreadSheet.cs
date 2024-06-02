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
    public int curStageID;

    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    public List<SkillScript> skillScripts = new();
    private List<Skill> skillLists = new();

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Load(Action callBack = default)
    {
        StartCoroutine(LoadData(0, ParseSkillData, callBack));
        StartCoroutine(LoadData(1, ParseTextData));
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

            var splitExplain = columns[6].Split('&');
            string explain = string.Join("\n", splitExplain);
            var newSkill = new Skill();
            newSkill.index = i;
            newSkill.skillName = columns[2];
            newSkill.cost = new int[3];
            newSkill.minDamage = new int[3];
            newSkill.maxDamage = new int[3];
            newSkill.attackCount = int.Parse(columns[5]);
            newSkill.effect = skillScripts[int.Parse(columns[0])];
            newSkill.keyIndex = int.Parse(columns[1]) - 1;
            newSkill.actionType = columns[3].EnumParse<Unit.ActionType>();
            newSkill.propertyType = columns[4].EnumParse<Unit.PropertyType>();
            newSkill.animationName = columns[7];
            newSkill.explain = explain;
            newSkill.icon = Resources.Load<Sprite>($"Icon/skill{int.Parse(columns[0])}");
            for (int j = 0; j < 3; j++)
            {
                newSkill.cost[j] = int.Parse(columns[8 + (j * 3)]);
                newSkill.minDamage[j] = int.Parse(columns[9 + (j * 3)]);
                newSkill.maxDamage[j] = int.Parse(columns[10 + (j * 3)]);
            }
            skillLists.Add(newSkill);
            skillDatas[keyCode].Add(newSkill);
        }
        d.skillData = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        d.SkillList = new List<Skill>(skillLists);
        Debug.Log("ReadEnd");
        //controller.inputs = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        //controller.inputLists = new List<Skill>(skillLists);
    }
    public void ParseTextData(string data)
    {
        if(curStageID == 0){
            Debug.Log("Don't ReadDialogue");
            return;
        }else Debug.Log("ReadDialogue");
        

        Queue<Dialogue> dialogBox = new();
        Queue<Queue<Dialogue>> hpDialogBox = new();

        Queue<Dialogue> act = new();

        var d = DataManager.instance;
        string[] rows = data.Split('\n');
        string nowDialogueType = null;
        bool isPlayer = false;
        Debug.Log($"rows.Length == {rows.Length}");
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            //Only text with the same CurStageID and stageID is imported from the sheet << Papago GO
            if (int.Parse(columns[0]) != curStageID || columns[0] == "") continue;

            if (columns[1] != "" && act.Count != 0)
            {
                if (nowDialogueType == "StoryDialogue") dialogBox = act;
                else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

                act.Clear();
            }

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

        if (nowDialogueType == "StoryDialogue") dialogBox = act;
        else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

        d.curStageDialogBox = dialogBox;
        d.hpDialogBox = new Queue<Queue<Dialogue>>(hpDialogBox);
        d.isPlayer = isPlayer;

        d.readEnd = true;
    }
}