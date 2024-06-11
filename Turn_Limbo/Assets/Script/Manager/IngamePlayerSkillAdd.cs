using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngamePlayerSkillAdd : MonoBehaviour
{
    private Controller controller;
    private void Start()
    {
        controller = GetComponent<Controller>();
        GivePlayerSkill();
    }
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();

    public void GivePlayerSkill()
    {
        var d = DataManager.instance;
        for (int i = 0; i < controller.player.skillInfo.SelectIndex.Count; i++)
        {
            var skill = d.loadData.SkillList[controller.player.skillInfo.SelectIndex[i]];
            controller.inputLists.Add(skill);
            int keyCode = skill.keyIndex;
            if (!controller.inputs.ContainsKey(keyCode))
                controller.inputs.Add(keyCode, new List<Skill>());

            controller.inputs[keyCode].Add(skill);
        }
        controller.InitEnemy();
        controller.InitBtn();
        controller.talkUnit = DataManager.instance.isPlayer ? controller.player : controller.enemy;
        DataManager.instance.InitUnit(controller.talkUnit);

        controller.isGame = true;
    }
}
