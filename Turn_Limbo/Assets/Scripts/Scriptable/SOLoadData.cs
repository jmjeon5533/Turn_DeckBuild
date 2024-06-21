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
public class ActionInfo
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

    public static ActionInfo Parse(string row)
    {
        string[] elements = row.Split('\t');
        var newSkill = new ActionInfo();
        int count = 0;
        newSkill.key = elements[count++];
        newSkill.inputKeyIndex = int.Parse(elements[count++]) - 1;
        newSkill.actionName = elements[count++];
        newSkill.actionType = elements[count++].EnumParse<ActionType>();
        newSkill.script = string.IsNullOrEmpty(elements[count]) ? "Base" : elements[count++];
        newSkill.actionDesc = elements[count++];
        newSkill.effectDesc = elements[count++];
        newSkill.sale = int.Parse(elements[19]);

        newSkill.requireAct = new int[4];
        newSkill.minDamage = new int[4];
        newSkill.maxDamage = new int[4];

        for (int i = 0; i < 4; i++)
        {
            newSkill.requireAct[i] = int.Parse(elements[7 + (i * 3)]);
            newSkill.minDamage[i] = int.Parse(elements[8 + (i * 3)]);
            newSkill.maxDamage[i] = int.Parse(elements[9 + (i * 3)]);
        }

        newSkill.icon = Resources.Load<Sprite>($"Icon/{newSkill.key}");

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

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class SOLoadData : ScriptableObject
{
    [SerializeField] private List<ActionInfo> actionInfos = new();
    [SerializeField] private List<ScenarioInfo> scenarioInfos = new();
    [SerializeField] private List<EnemyInfo> enemyInfos = new();

    public IReadOnlyDictionary<string, ActionInfo> ActionInfos { get; private set; }
    public IReadOnlyList<ScenarioInfo> ScenarioInfos => scenarioInfos;

    public void RuntimeInitialize()
    {
        var dict = new Dictionary<string, ActionInfo>();
        foreach(var info in actionInfos)
            dict.Add(info.key, info);
        ActionInfos = dict;
    }

#if UNITY_EDITOR
    public async UniTaskVoid Load()
    {
        Debug.Log("Loading...");
        //skill data
        await LoadData(1705787959, tsv =>
        {
            actionInfos.Clear();
            var cols = tsv.Split('\n');
            for (int i = 1; i < cols.Length; i++)
            {
                var newSkill = ActionInfo.Parse(cols[i]);
                actionInfos.Add(newSkill);
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
