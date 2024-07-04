using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public TreeNode parentNode;
    public List<TreeNode> childNode = new();

    public string name;
    public string desc;
    public string use;
    public int value;
    public int cost;

    public bool isOpen;

    PlusStats plusStats = DataManager.instance.saveData.plusStats;
}

// public class SkillTree
// {

// }