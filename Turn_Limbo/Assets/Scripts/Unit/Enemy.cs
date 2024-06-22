using System;
using UnityEngine;

public class Enemy : Unit
{
    private DataManager dataManager => DataManager.instance;

    protected string key;

    protected EnemyData Data => dataManager.loadData.EnemyDatas[key];

    public virtual void SetEnemyKey(string key)
    {
        this.key = key;
        for(int i = 0; i < Data.gainSkills.Length; i++)
        {
            var actionInfos = dataManager.loadData.ActionDatas[Data.gainSkills[i]];
            if (!actionTable.ContainsKey(actionInfos.script))
                actionTable.Add
                (
                    actionInfos.script,
                    Activator.CreateInstance(Type.GetType("Action_" + actionInfos.script), actionInfos, this, Data.gainSkillLvs[i]) as Action_Base
                );
        }
    }

    protected override void Start()
    {
        base.Start();

        hp = maxHP;
        shield = maxShield;
    }
}