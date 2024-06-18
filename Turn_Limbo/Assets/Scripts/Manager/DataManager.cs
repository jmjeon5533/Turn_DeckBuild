using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SaveData
{
    public int money;
    public int[] skillLv;
    public List<int> deckSkills = new List<int>();
    public List<int> skillLevels = new List<int>();
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

    //so
    public SOLoadData loadData;
    public SOInspectorData inspectorData;

    public int curStage;
    public SaveData saveData;
}
