using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameSkillAdd : MonoBehaviour, IInitObserver
{
    [SerializeField] private Controller controller;
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();

    public int Priority => 0;

    public void Init()
    {
        GivePlayerSkill(() =>
        {
            controller.SetStage();
            controller.TurnReset();
            UIManager.instance.SetExplain(false);
            GiveEnemySkill();
            controller.isGame = true;
        });
    }
    public void GivePlayerSkill(Action action)
    {
        var d = DataManager.instance;
        for (int i = 0; i < controller.player.skillInfo.selectIndex.Count; i++)
        {
            var skill = d.loadData.SkillList[controller.player.skillInfo.selectIndex[i]];
            controller.inputLists.Add(skill);
            int keyCode = skill.keyIndex;
            if (!controller.inputs.ContainsKey(keyCode))
                controller.inputs.Add(keyCode, new List<Skill>());

            controller.inputs[keyCode].Add(skill);
        }
        controller.InitBtn();
        controller.talkUnit = DataManager.instance.hpUnitIsPlayer ? controller.player : controller.enemy;
        DataManager.instance.InitDialog();
        DataManager.instance.InitUnit(controller.talkUnit);

        action?.Invoke();
    }
    public void GiveEnemySkill()
    {
        var skillList = controller.enemy.skillInfo;
        for(int i = 0; i < skillList.selectIndex.Count; i++)
        {
            var newSkill = new HoldSkills()
            {
                holdIndex = i,
                level = 0
            };
            skillList.holdSkills.Add(skillList.selectIndex[i], newSkill);
        }
    }
}
