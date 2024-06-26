using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Skills", order = 0)]
public class SkillEffect : ScriptableObject
{
    public List<int> selectIndex = new List<int>();
    public Dictionary<int,HoldSkills> holdSkills = new Dictionary<int,HoldSkills>();
}
[System.Serializable]
public class HoldSkills
{
    public int holdIndex;
    public int level;
}