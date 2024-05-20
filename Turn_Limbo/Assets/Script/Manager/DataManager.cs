using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Money;
}
public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
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
        if(data != string.Empty || data != "") saveData = JsonUtility.FromJson<SaveData>(data);
        else saveData = new SaveData();
    }
    public void JsonSave()
    {
        var data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData",data);
    }
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();
    public SaveData saveData;

    public Queue<Queue<Dialogue>> curStageDialogBox = new();
    public Queue<Queue<Dialogue>> hpDialogBox = new();
    public bool isPlayer;

    public void InitUnit(Unit unit){
        Debug.Log($"{hpDialogBox.Count} {(hpDialogBox.Count != 0 ? hpDialogBox.Peek().Count : -1)}");
        if(hpDialogBox.Count == 0) return;

        Debug.Log("test");
        unit.hpLimit = hpDialogBox.Peek().Peek().hpValue;
        unit.isDialogue = false;
    }
}
