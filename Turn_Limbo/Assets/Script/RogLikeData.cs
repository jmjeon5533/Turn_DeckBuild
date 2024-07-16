using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RogData", menuName = "Roglike", order = 4)]
public class RogLikeData : ScriptableObject
{
    public List<RogStageData> stageDatas = new();
}
[System.Serializable]
public class RogStageData
{
    public Sprite icon;
    public GameObject Bg;
    public string stageName;
    public int[] getSkillIndex;
    public Enemy[] spawnEnemyList;
    public Enemy spawnBoss;
    [Space(10)]
    public int clearGetMoney;
}