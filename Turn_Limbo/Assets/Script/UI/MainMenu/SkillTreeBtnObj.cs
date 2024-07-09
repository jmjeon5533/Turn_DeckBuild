using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeBtnObj : MonoBehaviour
{
    [SerializeField] List<Transform> line = new();
    [SerializeField] private Button skillTreeSelectBtn;
    private int[] lineStack = new int[3] { 0, 0, 0};

    public Button AddButton(int curLine, bool isParent = false)
    {
        Button btn;
        Transform curPos = line[curLine];
        if (!isParent)
        {
            btn = Instantiate(skillTreeSelectBtn, curPos);

            Vector3 plusPos = 
            new((lineStack[curLine] % 2 == 0 ? 0 : 80) * (lineStack[curLine] > 1 ? -1 : 1), 
            (lineStack[curLine] % 2 == 0 ? 80 : 0) * (lineStack[curLine] > 1 ? -1 : 1));

            btn.transform.localPosition += plusPos;
            btn.transform.localScale = btn.transform.localScale * 0.5f;

            lineStack[curLine]++;
            return btn;
        }


        btn = Instantiate(skillTreeSelectBtn, curPos);

        if (isParent) line[curLine] = btn.transform;

        return btn;
    }
}
