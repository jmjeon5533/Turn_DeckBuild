using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public const string ADDRESS = "https://docs.google.com/spreadsheets/d/1ENYCDg5E6WuUwf-NZjCOpJfRufJsxQI8d7qEKh3Kf_I";
    public readonly long[] SHEET_ID = { 1705787959};

    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    public List<SkillScript> skillScripts = new();
    public List<Skill> skillLists = new();

    [SerializeField] private Controller controller;

    private void Start()
    {
        StartCoroutine(LoadData(0, ParseSkillData));
        // StartCoroutine(LoadData(0, ParseEnemyData));
    }
    private IEnumerator LoadData(int pageIndex, Action<string> dataAction)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetCSVAddress(SHEET_ID[pageIndex]));
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        dataAction?.Invoke(data);
    }

    public static string GetCSVAddress(long sheetID)
    {
        return $"{ADDRESS}/export?format=csv&gid={sheetID}";
    }

    public void ParseSkillData(string data)
    {
        var d = DataManager.instance;
        string[] rows = data.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {   
            string[] columns = rows[i].Split(',');
            KeyCode keyCode = columns[1].EnumParse<KeyCode>();
            if (!skillDatas.ContainsKey(keyCode))
                skillDatas.Add(keyCode, new List<Skill>());

            var splitExplain = columns[9].Split('&');
            string explain = string.Join("\n",splitExplain);
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
        d.GivePlayerSkill();
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
}
