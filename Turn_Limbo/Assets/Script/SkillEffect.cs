using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Skills", order = 0)]
public class SkillEffect : ScriptableObject
{
    [Tooltip("선택한 스킬")]
    public List<int> SelectIndex = new List<int>();
    public List<holdSkills> holdSkills = new List<holdSkills>();
}
[System.Serializable]
public class holdSkills
{
    [Tooltip("보유한 스킬")]
    public int holdIndex;
    public int level;
}