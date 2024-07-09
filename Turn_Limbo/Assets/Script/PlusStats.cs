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

    public void AddStats(Unit unit){
        unit.maxHP += maxHP;
        unit.hp = unit.maxHP;
        unit.maxShield += maxShield;
        unit.shield = unit.maxShield;

        unit.plusAttackValue += plusAttackValue;
        unit.plusDefenseValue += plusDefenseValue;
        unit.attack_Drainage += attack_Drainage;
        unit.defense_Drainage += defense_Drainage;

        if(unit.TryGetComponent<Player>(out var p)){
            p.addCoin += addCoin;   
        }
    }

    public void Init()
    {
        maxHP = 0;
        maxShield = 0;
        plusAttackValue = 0;
        plusDefenseValue = 0;
        attack_Drainage = 0;
        defense_Drainage = 0;
        addCoin = 0;
    }
}