using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public static ReadSpreadSheet instance;
    public const string ADDRESS = "https://docs.google.com/spreadsheets/d/1ENYCDg5E6WuUwf-NZjCOpJfRufJsxQI8d7qEKh3Kf_I";
    public readonly long[] SHEET_ID = { 1705787959, 232901544, 930614922, 520277150, 2026427493 };

    [SerializeField] private TextAsset skill;
    [SerializeField] private TextAsset buff;
    [SerializeField] private TextAsset scenario;
    [SerializeField] private TextAsset skillTree;
    [SerializeField] private TextAsset EnemyData;
    //[SerializeField] private TextAsset EnemySkill;

    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    private List<Skill> skillLists = new();

    public bool isFirstLoad;

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
            StartCoroutine(LoadData(0, ParseSkillData));
            StartCoroutine(LoadData(1, PasreBuffData));
            StartCoroutine(LoadData(2, ParseTextData));
            StartCoroutine(LoadData(3, ParseEnemyData));
            StartCoroutine(LoadData(4, ParseSkillTreeData, callBack));
        }
        else
        {
            Debug.Log("IS NOT CONNECTION");
            LoadData(skill.text, ParseSkillData);
            LoadData(buff.text, PasreBuffData);
            LoadData(scenario.text, ParseTextData);
            LoadData(EnemyData.text, ParseEnemyData);
            //LoadData(EnemySkill.text, ParseEnemySkill);
            DataManager.instance.readEnd = true;
            callBack?.Invoke();
        }
    }

    private void LoadData(string csv, Action<string> action)
    {
        action?.Invoke(csv);
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
            newSkill.cost = new int[4];
            newSkill.minDamage = new int[4];
            newSkill.maxDamage = new int[4];
            newSkill.attackCount = int.Parse(columns[5]);
            newSkill.keyIndex = int.Parse(columns[1]) - 1;
            newSkill.actionType = columns[3].EnumParse<Unit.ActionType>();
            newSkill.propertyType = columns[4].EnumParse<PropertyType>();
            newSkill.animationName = columns[4];
            newSkill.effect_desc = explain;
            newSkill.sale = int.Parse(columns[22]);
            newSkill.isOnlyEnemy = bool.Parse(columns[24]);
            newSkill.skill_desc = columns[7];
            newSkill.icon = Resources.Load<Sprite>($"Icon/skill{int.Parse(columns[0])}");

            string className = "Skill_" + columns[6];
            try
            {
                newSkill.effect = Activator.CreateInstance(Type.GetType(className)) as SkillScript;
            }
            catch { Debug.LogError("NewSkillData"); newSkill.effect = Activator.CreateInstance(Type.GetType("Skill_")) as SkillScript; }


            for (int j = 0; j < 4; j++)
            {
                newSkill.cost[j] = int.Parse(columns[10 + (j * 3)]);
                newSkill.minDamage[j] = int.Parse(columns[11 + (j * 3)]);
                newSkill.maxDamage[j] = int.Parse(columns[12 + (j * 3)]);
            }
            skillLists.Add(newSkill);
            skillDatas[keyCode].Add(newSkill);
        }
        d.loadData.skillData = new SerializableDictionary<KeyCode, List<Skill>>(skillDatas);
        d.loadData.SkillList.Clear();
        d.loadData.SkillList = new List<Skill>(skillLists);
        Debug.Log("ReadEnd");
        //controller.inputs = new Dictionary<KeyCode, List<Skill>>(skillDatas);
        //controller.inputLists = new List<Skill>(skillLists);
    }

    void PasreBuffData(string data)
    {
        Debug.Log("ReadBuff");

        SerializableDictionary<string, BuffScript> buff = new();
        SerializableDictionary<string, BuffScript> debuff = new();

        var d = DataManager.instance;
        string[] rows = data.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            string className = "Buff_" + columns[2];
            var temp = Activator.CreateInstance(Type.GetType(className)) as BuffScript;
            temp.timing = columns[3].EnumParse<BuffTiming>();
            temp.buffIcon = Resources.Load<Sprite>($"BuffIcon/{columns[2]}");

            if (columns[4] == "buff" && columns[4] != string.Empty)
                buff.Add(columns[2], temp);
            else
                debuff.Add(columns[2], temp);
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

            if (columns[1] != string.Empty) nowDialogueType = columns[1];

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
                eventValue = columns[8] != string.Empty ? columns[8] : string.Empty,
            };

            //Debug.Log(newText.text);

            if (nowDialogueType == "HpDialogue" && columns[9] != string.Empty)
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

            if (nextColumns[1] != string.Empty && act.Count != 0)
            {
                if (nowDialogueType == "StoryDialogue") dialogBox = new Queue<Dialogue>(act);
                else hpDialogBox.Enqueue(new Queue<Dialogue>(act));

                act.Clear();
            }
            if (stageIndex != int.Parse(nextColumns[0]) && nextColumns[0] != string.Empty)
            {
                d.loadData.stageDialogBox.Add(stageIndex, new Queue<Dialogue>(dialogBox));
                d.loadData.hpDialogBox.Add(stageIndex, new Queue<Queue<Dialogue>>(hpDialogBox));

                stageIndex = int.Parse(nextColumns[0]);
            }
        }

        d.readEnd = true;
        d.hpUnitIsPlayer = isPlayer;
    }

    public void ParseSkillTreeData(string data)
    {
        var d = DataManager.instance;

        //if (data == skillTree.text) return;
        Debug.Log("ReadSkillTree");

        string[] rows = data.Split('\n');
        string[] startPoint = rows[1].Split(',');

        string startClassName = "SkillTree_" + startPoint[4];
        TreeNode startNode = new()
        {
            parentNode = new(){isOpen = true},
            name = startPoint[2],
            desc = startPoint[7],
            use = Activator.CreateInstance(Type.GetType(startClassName)) as SkillTreeScript,
            value = startPoint[5],
            cost = int.Parse(startPoint[6]),
            plusStats = d.plusStats,
            isOpen = d.saveData.treeData != null && d.saveData.treeData.startNode,
        };

        TreeNode lineOne = null;
        TreeNode lineTwo = null;
        TreeNode lineThree = null;

        int[] saveParentIndex = new int[4] { -1, 0, 0, 0 };
        int[] saveChildIndex = new int[4] { -1, 0, 0, 0 };
        int[] lineIndex = new int[4] { -1, 0, 0, 0 };
        int oldLine = 0;

        List<List<bool>> parentSaveData = new();
        List<List<bool>> childSaveData = new();
        if(d.saveData.treeData != null){
            parentSaveData = d.saveData.treeData.parent;
            childSaveData = d.saveData.treeData.child;
        }
        d.plusStats.Init();

        void SettingParent(TreeNode curNode, int curLine)
        {
            TreeNode parentNode = null;

            switch (curLine)
            {
                case 1: parentNode = lineOne; lineOne = curNode; break;
                case 2: parentNode = lineTwo; lineTwo = curNode; break;
                case 3: parentNode = lineThree; lineThree = curNode; break;
            }
            parentNode ??= startNode;

            curNode.parentNode = parentNode;
            curNode.isParent = true;
            curNode.lineNum = curLine;
            if(d.saveData.treeData != null) curNode.isOpen = parentSaveData[curLine - 1][saveParentIndex[curLine]];
            saveParentIndex[curLine]++;
            parentNode.childNode.Add(curNode);
        }

        void SettingChild(TreeNode curNode, int curLine)
        {
            TreeNode parentNode = GetNode(startNode.childNode[curLine - 1], lineIndex[curLine]);

            curNode.parentNode = parentNode;
            curNode.lineNum = curLine;
            if(d.saveData.treeData != null) curNode.isOpen = childSaveData[curLine -1][saveChildIndex[curLine]];
            saveChildIndex[curLine]++;
            parentNode.childNode.Add(curNode);
        }

        TreeNode GetNode(TreeNode curNode, int count)
        {
            if (count == 0) return curNode;
            else return GetNode(curNode.childNode[0], count - 1);
        }

        for (int i = 2; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split(',');

            int.TryParse(columns[1], out int readLine);
            string type = columns[3];
            string className = "SkillTree_" + columns[4];
            int.TryParse(columns[6], out int cost);

            TreeNode newTree = new()
            {
                name = columns[2],
                desc = columns[7],
                use = Activator.CreateInstance(Type.GetType(className)) as SkillTreeScript,
                value = columns[5],
                cost = cost,
                plusStats = d.plusStats,
            };

            if (type == "Parent") SettingParent(newTree, readLine);
            else if (type == "Child")
            {
                if (readLine != 0)
                {
                    lineIndex[oldLine]++;
                    //Debug.Log(lineIndex[oldLine] + " / " + readLine);
                    oldLine = readLine;
                }
                SettingChild(newTree, oldLine);
            }
            else lineIndex[oldLine]++;

            oldLine = readLine == 0 ? oldLine : readLine;
        }

        d.startNode = startNode;

        // void ReadTest(TreeNode test)
        // {
        //     foreach (var n in test.childNode)
        //     {
        //         Debug.Log($"cur : {n.desc}\n\tparent : {n.parentNode.desc}");
        //     }
        //     if(test.childNode.Count > 0) ReadTest(test.childNode[0]);
        // }

        // for (int i = 0; i < 3; i++)
        // {
        //     Debug.Log($"cur : {startNode.childNode[i].desc}\n\tparent : {startNode.childNode[i].parentNode.desc}");
        //     ReadTest(startNode.childNode[i]);
        //     Debug.Log("////////////////////////////////////");
        // }
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

    public void ParseEnemySkill(string data)
    {
        return;
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