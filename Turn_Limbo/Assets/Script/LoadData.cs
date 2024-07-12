using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class LoadData : ScriptableObject
{
    public SerializableDictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();
    public List<SpawnData> SpawnData = new();
    public List<Enemy> allEnemys = new();
    public List<Enemy> allBoss = new();

    public SerializableDictionary<string, BuffScript> buffList = new();
    public SerializableDictionary<string, BuffScript> debuffList = new();

    public List<UnitData> enemyData = new();

    public SerializableDictionary<int, Queue<Dialogue>> stageDialogBox = new();
    public SerializableDictionary<int, Queue<Queue<Dialogue>>> hpDialogBox = new();

}
