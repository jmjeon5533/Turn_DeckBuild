using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillExplain : MonoBehaviour
{
    [SerializeField] Image skillIcon;
    [SerializeField] Text skillName, skillExplain;
    public void ExplainSet(Sprite icon, string name, string explain)
    {
        skillIcon.sprite = icon;
        skillName.text = name;
        skillExplain.text = explain;
    }
}
