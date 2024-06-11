using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Money;
}
public struct UnitData
{
    public int index;
    public string name;
    public int hp;
    public int shield;
    public int atk;
    public int minCount;
    public int maxCount;
}
[System.Serializable]
public class SpawnData
{
    public SpriteRenderer maps;
    public List<Enemy> enemies = new();
}
public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        JsonLoad();
    }
    public void JsonLoad()
    {
        var data = PlayerPrefs.GetString("SaveData");
        if (data != string.Empty || data != "") saveData = JsonUtility.FromJson<SaveData>(data);
        else saveData = new SaveData();
    }
    public void JsonSave()
    {
        var data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData", data);
    }
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();
    public SaveData saveData;

    public List<UnitData> enemyData = new();
    public List<SpawnData> SpawnData = new();

    // public List<Buff_Base> buffList;
    // public List<Buff_Base> debuffList;

    public Dictionary<string, Buff_Base> buffList;
    public Dictionary<string, Buff_Base> debuffList;

    public Queue<Dialogue> curStageDialogBox = new();
    public Queue<Queue<Dialogue>> hpDialogBox = new();
    public bool isPlayer;
    public bool readEnd;

    public void InitUnit(Unit unit)
    {
        //Debug.Log($"{hpDialogBox.Count} {(hpDialogBox.Count != 0 ? hpDialogBox.Peek().Count : -1)}");
        if (hpDialogBox.Count == 0) return;

        Debug.Log("InitUnit");
        unit.hpLimit = hpDialogBox.Peek().Peek().hpValue;
        unit.isDialogue = false;
    }
}
