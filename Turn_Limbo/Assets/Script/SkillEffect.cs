using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Skills", order = 0)]
public class SkillEffect : ScriptableObject
{
    [Tooltip("?Ñ†?Éù?ïú ?ä§?Ç¨")]
    public List<int> selectIndex = new List<int>();
    public List<HoldSkills> holdSkills = new List<HoldSkills>();
}
[System.Serializable]
public class HoldSkills
{
    [Tooltip("Î≥¥Ïú†?ïú ?ä§?Ç¨")]
    public int holdIndex;
    public int level;
}