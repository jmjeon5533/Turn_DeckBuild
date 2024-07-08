using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeBtnObj : MonoBehaviour
{
    [SerializeField] List<Transform> line = new();
    [SerializeField] private Button skillTreeSelectBtn;

    public Button AddButton(int curLine, TreeNode curNode, bool isParent = false)
    {
        if(!isParent) return null;
        
        Transform curPos = line[curLine];

        Button btn = Instantiate(skillTreeSelectBtn, curPos);

        if(isParent) line[curLine] = btn.transform;

        return btn;
    }
}
