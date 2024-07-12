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
            controller.useTurnCount = 1;
            controller.SetStage();
            controller.TurnReset();
            UIManager.instance.SetExplain(false);
            controller.GiveEnemySkill();
            controller.isGame = true;
        });
    }
    public void GivePlayerSkill(Action action)
    {
        var d = DataManager.instance;

        bool RogMode = d.curMode == Controller.Modes.rogLike;
        var count = RogMode ? 9 : controller.player.skillInfo.selectIndex.Count;
        for (int i = 0; i < count; i++)
        {
            var skill = RogMode ? d.loadData.SkillList[i] : d.loadData.SkillList[controller.player.skillInfo.selectIndex[i]];
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
        foreach(var input in controller.inputs)
            foreach(var values in input.Value) print(values.index);

        action?.Invoke();
    }
}
