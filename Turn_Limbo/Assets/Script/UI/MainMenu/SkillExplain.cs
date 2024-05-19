using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkillExplain : MonoBehaviour
{
    [SerializeField] Image skillIcon;
    [SerializeField] Text skillName, skillExplain;
    [SerializeField] Text dmgText;
    public void ExplainSet(Skill skill)
    {
        skillIcon.sprite = skill.icon;
        StringBuilder sb = new StringBuilder();
        sb.Append(skill.skillName);
        // for(int i = 0; i < skill.level; i++)
        // {
        //     sb.Append("i");
        // }
        skillName.text = sb.ToString();
        skillExplain.text = skill.explain;
        dmgText.text = $"{skill.minDamage[skill.level]} ~ {skill.maxDamage[skill.level]}";
    }
}
