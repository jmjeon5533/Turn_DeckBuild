using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngamePlayerSkillAdd : MonoBehaviour
{
    private Controller controller;
    private void Start()
    {
        controller = GetComponent<Controller>();
        GivePlayerSkill(() =>
        {
            controller.SetStage();
            controller.TurnReset();
            UIManager.instance.SetExplain(false);
        });
    }
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();

    public void GivePlayerSkill(Action action)
    {
        var d = DataManager.instance;
        for (int i = 0; i < controller.player.skillInfo.SelectIndex.Count; i++)
        {
            var skill = d.loadData.SkillList[controller.player.skillInfo.SelectIndex[i]];
            // skill.level = controller.player.skillInfo.holdSkills
            //     [controller.player.skillInfo.holdSkills.FindIndex(x => x.holdIndex == skill.index)].level;
            controller.inputLists.Add(skill);
            int keyCode = skill.keyIndex;
            if (!controller.inputs.ContainsKey(keyCode))
                controller.inputs.Add(keyCode, new List<Skill>());

            controller.inputs[keyCode].Add(skill);
        }
        controller.InitEnemy();
        controller.InitBtn();
        controller.talkUnit = DataManager.instance.hpUnitIsPlayer ? controller.player : controller.enemy;
        DataManager.instance.InitDialog();
        DataManager.instance.InitUnit(controller.talkUnit);

        action?.Invoke();
        controller.isGame = true;
    }
}
