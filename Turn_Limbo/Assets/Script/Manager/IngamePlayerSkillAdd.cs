using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngamePlayerSkillAdd : MonoBehaviour
{
    private Controller controller;
    private void Start()
    {
        controller = GetComponent<Controller>();
        ReadSpreadSheet.instance.Load(GivePlayerSkill);
    }
    public SkillEffect[] skillEffects = new SkillEffect[2];
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();

    public Queue<Queue<Dialogue>> curStageDialogBox = new();
    public Queue<Queue<Dialogue>> hpDialogBox = new();
    public bool isPlayer;

    public void InitUnit(Unit unit){
        if(hpDialogBox.Count == 0) return;

        unit.hpLimit = hpDialogBox.Peek().Peek().hpValue;
        Debug.Log("test");
        unit.isDialogue = false;
    }

    public void GivePlayerSkill()
    {
        var d = DataManager.instance;
        for (int i = 0; i < d.skillEffects[0].holdIndex.Count; i++)
        {
            var skill = d.SkillList[d.skillEffects[0].holdIndex[i]];
            controller.inputLists.Add(skill);
            int keyCode = skill.keyIndex;
            if (!controller.inputs.ContainsKey(keyCode))
                controller.inputs.Add(keyCode, new List<Skill>());

            controller.inputs[keyCode].Add(skill);
        }
        controller.InitEnemy();
        controller.InitBtn();
        controller.talkUnit = isPlayer ? controller.player : controller.enemy;
        InitUnit(controller.talkUnit);

        controller.isGame = true;
    }
}
