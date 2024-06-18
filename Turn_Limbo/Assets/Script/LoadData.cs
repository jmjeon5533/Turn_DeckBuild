using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInfomation
{
    public string skillName;
    public int sale;
    public int[] cost;
    public int[] minDamage;
    public int[] maxDamage;
    public int attackCount;
    public KeyCode performKey;
    public Skill_Base skillInst;
    public Sprite icon;
    public Unit.ActionType actionType;
    public string effect_desc;
    public string skill_desc;
    public PropertyType propertyType;
}

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class LoadData : ScriptableObject
{
    public Dictionary<KeyCode, List<SkillInfomation>> skillData = new();
    public List<SkillInfomation> SkillList = new();
    public List<SpawnData> SpawnData = new();

    public Dictionary<string, Buff_Base> buffList;
    public Dictionary<string, Buff_Base> debuffList;

    public List<UnitData> enemyData = new();

    public Dictionary<int, Queue<Dialogue>> stageDialogBox = new();
    public Dictionary<int, Queue<Queue<Dialogue>>> hpDialogBox = new();

}
