using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SaveData
{
    public bool isInitialize;
    public int money;
    public List<int> selectIndex = new List<int>();
    public Dictionary<int, HoldSkills> holdSkills = new Dictionary<int, HoldSkills>();
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
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        JsonLoad();
    }
    private void JsonReset()
    {
        
    }
    public void JsonLoad()
    {
        var data = PlayerPrefs.GetString("SaveData");
        
        saveData = JsonConvert.DeserializeObject<SaveData>(data) ?? new SaveData();
        if(!saveData.isInitialize)
        {
            saveData.isInitialize = true;
            for(int i = 0; i < 6; i++)
                saveData.holdSkills.Add(i,new() { holdIndex = i, level = 0 });
        }
        player.holdSkills = saveData.holdSkills;
        player.selectIndex = saveData.selectIndex;
    }
    public void JsonSave()
    {
        // Debug.LogError(saveData.isInitialize);
        saveData.selectIndex = player.selectIndex;
        saveData.holdSkills = player.holdSkills;
        var data = JsonConvert.SerializeObject(saveData);
        PlayerPrefs.SetString("SaveData", data);
        PlayerPrefs.Save();
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            Debug.Log($"{loadData.buffList.Count} {loadData.debuffList.Count}");

        }
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
