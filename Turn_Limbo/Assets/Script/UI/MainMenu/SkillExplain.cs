using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkillExplain : MonoBehaviour
{
    [SerializeField] Image skillIcon;
    [SerializeField] Text skillName, skill_Desc, skill_Effect, skill_Cost, skill_BuyMoney, skill_keys;
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
        skill_Cost.text = skill.cost[level].ToString();
        if(skill_BuyMoney != null) skill_BuyMoney.text = $"АЁАн : {skill.sale * (level + 1)}";
        if(skill_keys != null) skill_keys.text = IndexToKey(skill.keyIndex);
    }
    string IndexToKey(int index)
    {
        string text = "";
        switch(index)
        {
            case 0 : text = "Q"; break;
            case 1 : text = "W"; break;
            case 2 : text = "E"; break;
            default : text = "X"; break;
        }
        return text;
    }
}
