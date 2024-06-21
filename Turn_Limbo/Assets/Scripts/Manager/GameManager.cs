using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

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
                if (!actionTable.ContainsKey(info.script))
                {
                    var instance = Activator.CreateInstance(Type.GetType("Action_" + info.script)) as Action_Base;
                    instance.Initialize(info);
                    actionTable.Add(info.script, instance);
                }
            }

        this
            .ObserveEveryValueChanged(x => x.attackRemainTime)
            .Where(x => x <= 0)
            .Skip(1)
            .Subscribe((x) => isSimulating = true);

        attackRemainTime = 10;

        this
            .ObserveEveryValueChanged(x => (int)x.attackRemainTime)
            .Subscribe(x => Debug.Log(x));

        StartCoroutine(GameRoutine());
    }

    private void Update()
    {
        if (attackRemainTime > 0)
        {
            attackRemainTime -= Time.deltaTime;
        }
    }

    private IEnumerator GameRoutine()
    {
        while(true)
        {
            player.OnTurnStart();
            enemy.OnTurnStart();
            
            yield return new WaitUntil(() => isSimulating);
            yield return StartCoroutine(SimulateRoutine());
            yield return new WaitForSeconds(1f);
            
            isSimulating = false;
        }
    }

    private IEnumerator SimulateRoutine()
    {
        Debug.Log("Simulate");
        isSimulating = true;
        while (player.ActionQueue.Count != 0 || enemy.ActionQueue.Count != 0)
        {
            player.PrepareAction();
            enemy.PrepareAction();

            //not one way attack
            if (player.HasAction && enemy.HasAction)
            {
                player.PerformAction(enemy);
                enemy.PerformAction(player);
                yield return new WaitUntil(() => enemy.IsEndAnimation && player.IsEndAnimation);
            }
            else //one way attack
            {
                Debug.Log(player.HasAction);
                if (player.HasAction)
                {
                    player.PerformAction(enemy, true);
                    yield return new WaitUntil(() => player.IsEndAnimation);
                }

                if (enemy.HasAction)
                {
                    enemy.PerformAction(player, true);
                    yield return new WaitUntil(() => enemy.IsEndAnimation);
                }
            }
            yield return null;
        }
        isSimulating = false;
        Debug.Log("End");
    }
}