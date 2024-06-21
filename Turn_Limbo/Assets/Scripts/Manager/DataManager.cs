using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public class ActionLevelData
    {
        public string key;
        public int lv;
    }

    [System.Serializable]
    public class DeckList
    {
        public List<string> actionList = new();
    }

    public int money;
    public List<DeckList> decks = new(); 
    public List<ActionLevelData> actionLevels = new List<ActionLevelData>();
}

[System.Serializable]
public class SpawnData
{
    public SpriteRenderer maps;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            loadData.RuntimeInitialize();

            for(int i = 0; i < saveData.decks.Count; i++)
            {
                deck[i] = new();
                foreach(var action in saveData.decks[i].actionList)
                {
                    Debug.Log(action);
                    deck[i].Add(action);
                }
            }

            foreach(var info in saveData.actionLevels)
                actionLevels.Add(info.key, info.lv);
        }
    }

    //so
    public SOLoadData loadData;
    public SOInspectorData inspectorData;

    //save
    public SaveData saveData;

    //runtime
    public int curStage;
    public List<string>[] deck = new List<string>[3];
    public Dictionary<string, int> actionLevels = new();
}
