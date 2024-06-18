using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inspectors", menuName = "Inspectors", order = 1)]
public class SOInspectorData : ScriptableObject
{
    public List<Sprite> backgroundSprites = new();
}