using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlusStats", menuName = "PlusStats", order = 2)]
public class PlusStats : ScriptableObject
{
    public int maxHP;
    public int maxShield;

    public int plusAttackValue;
    public int plusDefenseValue;
    public float attack_Drainage;
    public float defense_Drainage;

    public int addCoin;
}