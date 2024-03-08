using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Skills", order = 0)]
public class SkillEffect : ScriptableObject
{
    public List<int> holdIndex = new List<int>();
}
