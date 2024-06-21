using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    //singleton
    private DataManager dataManager => DataManager.instance;

    private int act;
    private Queue<string>[] keyActionTable = new Queue<string>[3];

    private KeyCode[] InputKeys => new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E };

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < 3; i++) keyActionTable[i] = new();
        foreach (var list in dataManager.deck)
            foreach (var actionKey in list)
            {
                var info = dataManager.loadData.ActionInfos[actionKey];
                keyActionTable[info.inputKeyIndex].Enqueue(actionKey);
            }
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetKeyDown(InputKeys[i]))
            {
                var key = keyActionTable[i].Peek();
                var info = dataManager.loadData.ActionInfos[key];
                var useAct = info.requireAct[dataManager.actionLevels[key]];

                if (act >= useAct)
                {
                    actionQueue.Add(key);
                    keyActionTable[i].Dequeue();
                    keyActionTable[i].Enqueue(key);

                    act -= useAct;
                }
            }
        }
    }
}