using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class LoadData : ScriptableObject
{
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();
    public List<SpawnData> SpawnData = new();

    public List<UnitData> enemyData = new();

    public Dictionary<int, Queue<Dialogue>> stageDialogBox = new();
    public Dictionary<int, Queue<Queue<Dialogue>>> hpDialogBox = new();

}
