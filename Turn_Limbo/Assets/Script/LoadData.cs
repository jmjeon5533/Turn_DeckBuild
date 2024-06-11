using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class LoadData : ScriptableObject
{
    public Dictionary<KeyCode, List<Skill>> skillDatas = new();
    public List<SkillScript> skillScripts = new();
    public List<Skill> skillLists = new();
}
