using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ActionData
{
    public string key;
    public int inputKeyIndex;
    public string actionName;
    public ActionType actionType;
    public string script;
    public string actionDesc;
    public string effectDesc;
    public int sale;
    public int[] requireAct;
    public int[] minDamage;
    public int[] maxDamage;
    public Sprite icon;

    public static ActionData Parse(string row)
    {
        string[] elements = row.Split('\t');
        var info = new ActionData();
        int count = 0;
        info.key = elements[count++];
        info.inputKeyIndex = int.Parse(elements[count++]) - 1;
        info.actionName = elements[count++];
        info.actionType = elements[count++].EnumParse<ActionType>();
        info.script = string.IsNullOrEmpty(elements[count]) ? "Base" : elements[count++];
        info.actionDesc = elements[count++];
        info.effectDesc = elements[count++];
        info.sale = int.Parse(elements[19]);

        info.requireAct = new int[4];
        info.minDamage = new int[4];
        info.maxDamage = new int[4];

        for (int i = 0; i < 4; i++)
        {
            info.requireAct[i] = int.Parse(elements[7 + (i * 3)]);
            info.minDamage[i] = int.Parse(elements[8 + (i * 3)]);
            info.maxDamage[i] = int.Parse(elements[9 + (i * 3)]);
        }

        info.icon = Resources.Load<Sprite>($"Icon/{info.key}");

        return info;
    }
}

[System.Serializable]
public class ScenarioData
{
    public DialogueData[] dialogues;

    public static ScenarioData Parse(string[] rows)
    {
        var stageTextInfo = new ScenarioData() { dialogues = new DialogueData[rows.Length] };
        for (int i = 0; i < rows.Length; i++)
            stageTextInfo.dialogues[i] = DialogueData.Parse(rows[i]);

        return stageTextInfo;
    }
}

[System.Serializable]
public class DialogueData
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
    //public Sprite icon;
    //effect
    //target
    //background

    public static DialogueData Parse(string row)
    {
        var elements = row.Split('\t');
        var count = 0;
        var info = new DialogueData();
        info.stageId = int.Parse(elements[count++]);
        info.dialogue = elements[count++];
        info.name = elements[count++];
        info.job = elements[count++];
        info.namePos = Enum.Parse<ScenarioManager.NamePos>(elements[count++]);
        info.camPos = Enum.Parse<ScenarioManager.CamPos>(elements[count++]);
        info.text = elements[count++];
        info.curEvent = Enum.Parse<ScenarioManager.DialogueEvent>(elements[count++]);
        info.eventValue = elements[count++];
        return info;
    }
}

[System.Serializable]
public class EnemyData
{
    public string key;
    public string name;
    public int maxHp;
    public int maxShield;
    public int atk;
    public int minActionCount;
    public int maxActionCount;
    public string[] gainSkills;
    public int[] gainSkillLvs;

    public static EnemyData Parse(string row)
    {
        var elements = row.Split('\t');
        var count = 0;
        var info = new EnemyData();
        info.key = elements[count++];
        info.name = elements[count++];
        info.maxHp = int.Parse(elements[count++]);
        info.maxShield = int.Parse(elements[count++]);
        info.atk = int.Parse(elements[count++]);
        info.minActionCount = int.Parse(elements[count++]);
        info.maxActionCount = int.Parse(elements[count++]);
        info.gainSkills = elements[count++].Split(',').ToArray();
        info.gainSkillLvs = elements[count++].Split(',').Select(x => int.Parse(x)).ToArray();
        return info;
    }
}

[Serializable]
public class StageData
{
    public string enemy;

    public static StageData Parse(string row)
    {
        var elements = row.Split('\t');
        var info = new StageData();
        info.enemy = elements[1];
        return info;
    }
}

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class SOLoadData : ScriptableObject
{
    [SerializeField] private List<ActionData> actionDatas = new();
    [SerializeField] private List<ScenarioData> scenarioDatas = new();
    [SerializeField] private List<EnemyData> enemyDatas = new();
    [SerializeField] private List<StageData> stageDatas = new();

    public IReadOnlyDictionary<string, ActionData> ActionDatas { get; private set; }
    public IReadOnlyList<ScenarioData> ScenarioDatas => scenarioDatas;
    public IReadOnlyDictionary<string, EnemyData> EnemyDatas { get; private set; }
    public IReadOnlyList<StageData> StageDatas => stageDatas;

    public void RuntimeInitialize()
    {
        var actionDatas = new Dictionary<string, ActionData>();
        foreach(var info in this.actionDatas)
            actionDatas.Add(info.key, info);
        ActionDatas = actionDatas;

        var enemyDatas = new Dictionary<string, EnemyData>();
        foreach(var info in this.enemyDatas)
            enemyDatas.Add(info.key, info);
        EnemyDatas = enemyDatas;
    }

#if UNITY_EDITOR
    public async UniTaskVoid Load()
    {
        Debug.Log("Loading...");
        //skill data
        await LoadData(1705787959, tsv =>
        {
            actionDatas.Clear();
            var cols = tsv.Split('\n');
            for (int i = 1; i < cols.Length; i++)
            {
                var newSkill = ActionData.Parse(cols[i]);
                actionDatas.Add(newSkill);
                // skillDatas[keyCode].Add(newSkill);
            }
        });
        Debug.Log("Skill Data Complete");

        //scenario data
        await LoadData(930614922, tsv =>
        {
            scenarioDatas.Clear();
            var split = tsv.Split('\n');
            var currentStage = '0';
            var start = 1;
            for (int i = 1; i < split.Length; i++)
            {
                //check if stage is different
                if (currentStage != split[i][0])
                {
                    scenarioDatas.Add(ScenarioData.Parse(split[start..i]));
                    start = i;
                    currentStage = split[i][0];
                }
            }
        });
        Debug.Log("Scenario Data Complete");

        //enemy data
        await LoadData(520277150, tsv =>
        {
            enemyDatas.Clear();
            var split = tsv.Split('\n');
            for (int i = 1; i < split.Length; i++)
                enemyDatas.Add(EnemyData.Parse(split[i]));
        });
        Debug.Log("Enemy Data Complete");

        //stage data
        await LoadData(77255081, tsv =>
        {
            stageDatas.Clear();
            var split = tsv.Split('\n');
            for(int i = 1; i < split.Length; i++)
                stageDatas.Add(StageData.Parse(split[i]));
        });

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
        
        Debug.Log("Loading Complete");
    }

    private async UniTask LoadData(int sheetID, Action<string> action)
    {
        using (var www = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/1RGa9VBqhPjvBIWskMJEESEHT03fKqcpSrQDT6jN8JHE/export?format=tsv&gid={sheetID}"))
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
