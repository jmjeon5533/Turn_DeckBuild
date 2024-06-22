using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private DataManager dataManager => DataManager.instance;

    private Player player;
    private Enemy enemy;
    private float attackRemainTime;
    private bool isSimulating;

    public float AttackRemainTime => attackRemainTime;
    public bool IsAttacking => isSimulating;

    private void Awake()
    {
        instance = this;
        
        player = Instantiate(Resources.Load<Player>("Prefabs/Player"), new Vector3(-5, 0, 0), Quaternion.identity);

        var enemyKey = dataManager.loadData.StageDatas[dataManager.curStage].enemy;
        enemy = Instantiate(Resources.Load<Enemy>($"Prefabs/Enemy/{enemyKey}"), new Vector3(5, 0, 0), Quaternion.identity);
        enemy.SetEnemyKey(enemyKey);
    }

    private void Start()
    {
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
                // player.PerformAction(enemy, );
                // enemy.PerformAction(player);
                yield return new WaitUntil(() => enemy.IsEndAnimation && player.IsEndAnimation);
            }
            else //one way attack
            {
                Debug.Log(player.HasAction);
                if (player.HasAction)
                {
                    // player.PerformAction(enemy, true);
                    yield return new WaitUntil(() => player.IsEndAnimation);
                }

                if (enemy.HasAction)
                {
                    // enemy.PerformAction(player, true);
                    yield return new WaitUntil(() => enemy.IsEndAnimation);
                }
            }
            yield return null;
        }
        isSimulating = false;
        Debug.Log("End");
    }
}