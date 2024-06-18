using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SaveData
{
    public int Money;
    public List<int> SelectIndex = new List<int>();
    public List<holdSkills> holdSkills = new List<holdSkills>();
}
[System.Serializable]
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
    public SaveData saveData;
    public LoadData loadData;
    public SkillEffect player;
    public SkillEffect enemy;
    public int curStageID;

    public Queue<Dialogue> stageDialogBox = new();
    public Queue<Queue<Dialogue>> hpDialogBox = new();

    public bool hpUnitIsPlayer;
    public bool readEnd;

    public void InitDialog()
    {
        if(loadData.stageDialogBox.TryGetValue(curStageID, out Queue<Dialogue> stage)) stageDialogBox = stage;
        
        if(loadData.hpDialogBox.TryGetValue(curStageID, out Queue<Queue<Dialogue>> hp)) hpDialogBox = hp;
    }

    public void InitUnit(Unit unit)
    {
        //Debug.Log($"{hpDialogBox.Count} {(hpDialogBox.Count != 0 ? hpDialogBox.Peek().Count : -1)}");
        if (hpDialogBox.Count == 0) return;

        Debug.Log("InitUnit");
        unit.hpLimit = hpDialogBox.Peek().Peek().hpValue;
        unit.isDialogue = false;
    }
}
