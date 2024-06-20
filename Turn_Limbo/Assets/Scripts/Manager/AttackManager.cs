using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance { get; private set; }

    private DataManager dataManager => DataManager.instance;

    [SerializeField] private Unit player;
    [SerializeField] private Unit enemy;

    private Dictionary<string, Action_Base> actionTable = new();

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
    }

    private IEnumerator Attack()
    {
        while (player.ActionQueue.Count != 0 || enemy.ActionQueue.Count != 0)
        {
            //not one way attack
            if(player.HasAction && enemy.HasAction)
            {
                var playerPerformResult = enemy.Simulate(player.GetPerformInfo());
                var enemyPerformResult = player.Simulate(enemy.GetPerformInfo());

                enemy.Attack();
                player.Attack();
                yield return new WaitUntil(() => enemy.IsEndAnimation && player.IsEndAnimation);

                enemy.Damage(playerPerformResult);
                player.Damage(enemyPerformResult);
            }
            else //one way attack
            {
                if(player.HasAction)
                {

                }
                else
                {

                }
            }

            yield return null;
        }
    }
}