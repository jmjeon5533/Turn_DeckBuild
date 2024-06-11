using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loads", menuName = "Loads", order = 1)]
public class LoadData : ScriptableObject
{
    public List<SpawnData> SpawnData = new();
}
