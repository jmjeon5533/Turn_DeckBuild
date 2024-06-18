using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkillExplain : MonoBehaviour
{
    [SerializeField] Image skillIcon;
    [SerializeField] Text skillName, skill_Desc, skill_Effect, skill_Cost, skill_BuyMoney;
    [SerializeField] Text dmgText;
    public void ExplainSet(Skill skill,int level)
    {
        skillIcon.sprite = skill.icon;
        StringBuilder sb = new StringBuilder();
        sb.Append(skill.skillName);
        for(int i = 0; i < level; i++)
        {
            sb.Append("+");
        }
        skillName.text = sb.ToString();
        skill_Desc.text = skill.skill_desc;
        skill_Effect.text = skill.effect_desc;
        dmgText.text = $"{skill.minDamage[level]} ~ {skill.maxDamage[level]}";
        skill_Cost.text = skill.cost[skill.level].ToString();
        if(skill_BuyMoney != null) skill_BuyMoney.text = $"АЁАн : {((level + 1) * 150).ToString()}";
    }
}
