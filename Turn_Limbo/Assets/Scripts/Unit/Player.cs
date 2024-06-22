using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player instance { get; private set; }

    //singleton
    private DataManager dataManager => DataManager.instance;

    private int act = 10;
    private List<string>[] deckQueues = new List<string>[3];

    private KeyCode[] InputKeys => new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E };

    public int Act => act;
    public IReadOnlyList<string>[] DeckQueues => deckQueues;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < 3; i++) deckQueues[i] = new();
        foreach (var list in dataManager.deck)
            foreach (var actionKey in list)
            {
                var info = dataManager.loadData.ActionDatas[actionKey];
                deckQueues[info.inputKeyIndex].Add(actionKey);
            }
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetKeyDown(InputKeys[i]))
            {
                var key = deckQueues[i][0];
                var info = dataManager.loadData.ActionDatas[key];
                var useAct = info.requireAct[dataManager.actionLevels[key]];

                if (act >= useAct)
                {
                    Debug.Log(key);
                    actionQueue.Add(key);
                    deckQueues[i].RemoveAt(0);
                    deckQueues[i].Add(key);

                    act -= useAct;
                }
            }
        }
    }
}