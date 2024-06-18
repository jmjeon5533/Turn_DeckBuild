using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SkillInfo
{
    public int index;
    public int keyIndex;
    public string skillName;
    public Unit.ActionType actionType;
    public PropertyType propertyType;
    public int attackCount;
    public string script;
    public string skill_desc;
    public string effect_desc;
    public string animationName;

    public int sale;
    public Sprite icon;
    public int[] act;
    public int[] minDamage;
    public int[] maxDamage;

    public static SkillInfo Parse(string row)
    {
        string[] elements = row.Split('\t');
        var newSkill = new SkillInfo();
        newSkill.index = int.Parse(elements[0]);
        newSkill.keyIndex = int.Parse(elements[1]) - 1;
        newSkill.skillName = elements[2];
        newSkill.actionType = elements[3].EnumParse<Unit.ActionType>();
        newSkill.propertyType = elements[4].EnumParse<PropertyType>();
        newSkill.attackCount = int.Parse(elements[5]);
        newSkill.script = elements[6];
        newSkill.skill_desc = elements[7];
        newSkill.effect_desc = elements[8];
        newSkill.animationName = elements[9];
        newSkill.icon = Resources.Load<Sprite>($"Icon/skill{newSkill.index}");

        newSkill.act = new int[4];
        newSkill.minDamage = new int[4];
        newSkill.maxDamage = new int[4];

        for (int j = 0; j < 4; j++)
        {
            newSkill.act[j] = int.Parse(elements[10 + (j * 3)]);
            newSkill.minDamage[j] = int.Parse(elements[11 + (j * 3)]);
            newSkill.maxDamage[j] = int.Parse(elements[12 + (j * 3)]);
        }

        return newSkill;
    }
}

[System.Serializable]
public class ScenarioInfo
{
    public DialogueInfo[] dialogues;

    public static ScenarioInfo Parse(string[] rows)
    {
        var stageTextInfo = new ScenarioInfo() { dialogues = new DialogueInfo[rows.Length] };
        for (int i = 0; i < rows.Length; i++)
            stageTextInfo.dialogues[i] = DialogueInfo.Parse(rows[i]);

        return stageTextInfo;
    }
}

[System.Serializable]
public class DialogueInfo
{
    public int stageId;
    public string dialogue;
    public string name;
    public string job;
    public string text;
    public ScenarioManager.NamePos namePos;
    public ScenarioManager.CamPos camPos;
    public ScenarioManager.DialogueEvent curEvent;
    public string eventValue;
    public string playerandenemy; //require change
    public int hpValue;
    //public Sprite icon;
    //effect
    //target
    //background

    public static DialogueInfo Parse(string row)
    {
        var split = row.Split('\t');
        var count = 0;
        var info = new DialogueInfo();
        info.stageId = int.Parse(split[count++]);
        info.dialogue = split[count++];
        info.name = split[count++];
        info.job = split[count++];
        info.namePos = Enum.Parse<ScenarioManager.NamePos>(split[count++]);
        info.camPos = Enum.Parse<ScenarioManager.CamPos>(split[count++]);
        info.text = split[count++];
        info.curEvent = Enum.Parse<ScenarioManager.DialogueEvent>(split[count++]);
        info.eventValue = split[count++];
        info.playerandenemy = split[count++];
        int.TryParse(split[count++], out info.hpValue);
        return info;
    }
}

[System.Serializable]
public class EnemyInfo
{
    public int index;
    public string name;
    public int hp;
    public int shield;
    public int atk;
    public int minCount;
    public int maxCount;

    public static EnemyInfo Parse(string row)
    {
        string[] column = row.Split('\t');
        var count = 0;
        var newEnemy = new EnemyInfo();
        newEnemy.index = int.Parse(column[count++]);
        newEnemy.name = column[count++];
        newEnemy.hp = int.Parse(column[count++]);
        newEnemy.shield = int.Parse(column[count++]);
        newEnemy.atk = int.Parse(column[count++]);
        newEnemy.minCount = int.Parse(column[count++]);
        newEnemy.maxCount = int.Parse(column[count++]);
        return newEnemy;
    }
}

[System.Serializable]
public class BuffInfo
{
    public int index;
    public string name;
    public string script;
    public string type;

    public static BuffInfo Parse(string row)
    {
        var split = row.Split('\t');
        var count = 0;
        var infos = new BuffInfo();
        infos.index = int.Parse(split[count++]);
        infos.name = split[count++];
        infos.script = split[count++];
        // infos.timing = Enum.Parse<BuffReduceTiming>(split[count++]);
        infos.type = split[count++];
        return infos;

        // string[] columns = row.Split('\t');

        // string className = "Buff_" + columns[2];
        // var temp = Activator.CreateInstance(Type.GetType(className)) as Buff_Base;
        // temp.timing = columns[3].EnumParse<BuffTiming>();
        // temp.buffIcon = Resources.Load<Sprite>($"BuffIcon/{columns[2]}");
        // //Debug.Log($"Icon/BuffIcon/{columns[2]}");

        // if (columns[4] == "buff" && columns[4] != "") buff.Add(columns[2], temp);
        // else debuff.Add(columns[2], temp);
    }
}

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class SOLoadData : ScriptableObject
{
    public List<SkillInfo> skillInfos = new();
    public List<ScenarioInfo> scenarioInfos = new();
    public List<EnemyInfo> enemyInfos = new();
    public List<BuffInfo> buffInfos = new();

#if UNITY_EDITOR
    public async UniTaskVoid Load()
    {
        Debug.Log("Loading...");
        //skill data
        await LoadData(1705787959, tsv =>
        {
            skillInfos.Clear();
            var cols = tsv.Split('\n');
            for (int i = 1; i < cols.Length; i++)
            {
                var newSkill = SkillInfo.Parse(cols[i]);
                skillInfos.Add(newSkill);
                // skillDatas[keyCode].Add(newSkill);
            }
        });
        Debug.Log("Skill Data Complete");

        //scenario data
        await LoadData(930614922, tsv =>
        {
            scenarioInfos.Clear();
            var split = tsv.Split('\n');
            var currentStage = '0';
            var start = 1;
            for (int i = 1; i < split.Length; i++)
            {
                //check if stage is different
                if (currentStage != split[i][0])
                {
                    scenarioInfos.Add(ScenarioInfo.Parse(split[start..i]));
                    start = i;
                    currentStage = split[i][0];
                }
            }
        });
        Debug.Log("Scenario Data Complete");

        //enemy data
        await LoadData(520277150, tsv =>
        {
            enemyInfos.Clear();
            var split = tsv.Split('\n');
            for (int i = 1; i < split.Length; i++)
                enemyInfos.Add(EnemyInfo.Parse(split[i]));
        });
        Debug.Log("Enemy Data Complete");

        //buff data
        await LoadData(232901544, tsv =>
        {
            buffInfos.Clear();
            var split = tsv.Split('\n');
            for (int i = 1; i < split.Length; i++)
                buffInfos.Add(BuffInfo.Parse(split[i]));
        });
        Debug.Log("Buff Data Complete");

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
        
        Debug.Log("Loading Complete");
    }

    private async UniTask LoadData(int sheetID, Action<string> action)
    {
        using (var www = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/1ENYCDg5E6WuUwf-NZjCOpJfRufJsxQI8d7qEKh3Kf_I/export?format=tsv&gid={sheetID}"))
        {
            await www.SendWebRequest();
            action?.Invoke(www.downloadHandler.text);
        }
    }

    [CustomEditor(typeof(SOLoadData))]
    public class LoadDataEditor : Editor
    {
        public SOLoadData loadData => target as SOLoadData;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load"))
                loadData.Load().Forget();
        }
    }
#endif
}
