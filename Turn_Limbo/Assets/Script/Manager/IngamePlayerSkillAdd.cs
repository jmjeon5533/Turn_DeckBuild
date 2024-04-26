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
        controller.isGame = true;
    }
}
