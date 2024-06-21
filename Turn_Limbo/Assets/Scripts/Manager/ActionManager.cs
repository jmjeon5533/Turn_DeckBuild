using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance { get; private set; }

    private DataManager dataManager => DataManager.instance;

    [SerializeField] private Unit player;
    [SerializeField] private Unit enemy;

    private float attackRemainTime;
    private bool isSimulating;
    private Dictionary<string, Action_Base> actionTable = new();

    public float AttackRemainTime => attackRemainTime;
    public bool IsAttacking => isSimulating;
    public IReadOnlyDictionary<string, Action_Base> ActionTable => actionTable;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var list in dataManager.deck)
            foreach (var actionKey in list)
            {
                var info = dataManager.loadData.ActionInfos[actionKey];
                if (!actionTable.ContainsKey(actionKey))
                    actionTable.Add(actionKey, Activator.CreateInstance(Type.GetType("Action_" + info.script)) as Action_Base);
            }

        this
            .ObserveEveryValueChanged(x => x.attackRemainTime)
            .Where(x => x <= 0)
            .Skip(0)
            .Subscribe((x) => Simulate());
        
        attackRemainTime = 10;
    }

    private void Update()
    {
        if(attackRemainTime > 0)
        {
            Debug.Log(attackRemainTime);
            attackRemainTime -= Time.deltaTime;
        }
    }

    public void Simulate()
    {
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            Debug.Log("Simulate");
            isSimulating = true;
            while (player.ActionQueue.Count != 0 || enemy.ActionQueue.Count != 0)
            {
                //not one way attack
                if (player.HasAction && enemy.HasAction)
                {
                    player.PerformAction(enemy);
                    enemy.PerformAction(player);
                    yield return new WaitUntil(() => enemy.IsEndAnimation && player.IsEndAnimation);
                }
                else //one way attack
                {
                    if (player.HasAction)
                    {
                        player.PerformAction(enemy, true);
                        yield return new WaitUntil(() => player.IsEndAnimation);
                    }
                    else
                    {
                        enemy.PerformAction(player, true);
                        yield return new WaitUntil(() => enemy.IsEndAnimation);
                    }
                }
            }
            isSimulating = false;
            Debug.Log("End");
        }
    }
}