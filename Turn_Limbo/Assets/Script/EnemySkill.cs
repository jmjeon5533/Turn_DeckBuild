using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "Skills", order = 0)]
public class EnemySkill : ScriptableObject
{
    public List<Skill> skills = new List<Skill>();
    public int requestCount = 4;
}
