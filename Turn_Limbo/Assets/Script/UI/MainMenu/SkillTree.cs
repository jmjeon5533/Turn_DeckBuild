using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TreeNode
{
    public TreeNode parentNode;
    public List<TreeNode> childNode = new();

    public string name;
    public string desc;
    public string use;
    public int value;
    public int cost;

    public bool isParent;
    public bool isOpen;

    PlusStats plusStats = DataManager.instance.saveData.plusStats;
}

public class SkillTree : MonoBehaviour
{
    [SerializeField] Transform panels;
    [SerializeField] Transform selectPanels;
    [SerializeField] private RectTransform skillTreeParent;
    [SerializeField] GameObject groupObj;
    public List<SkillTreeBtnObj> groups = new();
    public List<RectTransform> btn;

    int selectLine;
    int selectIndex;

    bool isShow = false;
    bool isPanelShow = false;

    [Header("SelectPanels")]
    [SerializeField] Text nameText;
    [SerializeField] Text desc;
    [SerializeField] Text cost;

    public void OnOffPanel()
    {
        isShow = !isShow;

        panels.gameObject.SetActive(isShow);
        //DataManager.instance.JsonSave();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            AddSkillTreeButton();
        }
    }

    public void AddSkillTreeButton()
    {
        var d = DataManager.instance;
        TreeNode startNode = d.startNode;

        void ReadButton(TreeNode curNode, int curLine, int count)
        {
            SkillTreeBtnObj parnetObj;

            if(groups.Count <= count){
                parnetObj = Instantiate(groupObj, skillTreeParent).GetComponent<SkillTreeBtnObj>();
                groups.Add(parnetObj);
            }else parnetObj = groups[count];

            if(curNode.isParent) SettingButton(curLine, parnetObj, curNode, true);

            for(int i = 0; i > curNode.childNode.Count; i++){
                if(i == 0) continue;
                SettingButton(curLine, parnetObj, curNode.childNode[i]);
            }
            if (curNode.childNode.Count > 0) ReadButton(curNode.childNode[0], curLine, count + 1);
        }

        void SettingButton(int curLine, SkillTreeBtnObj parnetObj, TreeNode curNode, bool isParent = false){
            Button btn;

            btn = parnetObj.AddButton(curLine, curNode, isParent);

            btn.onClick.AddListener(() =>{
                SettingSelectPanel(curNode);
            });
        }

        for (int i = 0; i < 3; i++)
        {
            ReadButton(startNode.childNode[i], i, 0);
        }
    }

    public void SettingSelectPanel(TreeNode curNode)
    {
        nameText.text = curNode.name;
        desc.text = curNode.desc;
        cost.text = curNode.cost.ToString();
    }
}
