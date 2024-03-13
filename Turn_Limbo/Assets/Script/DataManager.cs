using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    [SerializeField] Controller controller;
    private void Awake()
    {
        instance = this;
    }
    public SkillEffect[] skillEffects = new SkillEffect[2];
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();
    public void GivePlayerSkill()
    {
        for (int i = 0; i < skillEffects[0].holdIndex.Count; i++)
        {
            var skill = SkillList[skillEffects[0].holdIndex[i]];
            controller.inputLists.Add(skill);
            int keyCode = skill.keyIndex;
            if (!controller.inputs.ContainsKey(keyCode))
                controller.inputs.Add(keyCode, new List<Skill>());

            controller.inputs[keyCode].Add(skill);
        }
        controller.InitEnemy();
        controller.InitBtn();
    }
}
