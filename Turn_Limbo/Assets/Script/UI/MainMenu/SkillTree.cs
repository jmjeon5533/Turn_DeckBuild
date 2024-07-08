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
    public SkillTreeScript use;
    public string value;
    public int cost;

    public bool isParent;
    public bool isOpen;

    public PlusStats plusStats;
}

public class SkillTree : MonoBehaviour
{
    [SerializeField] Transform panels;
    [SerializeField] Transform selectPanels;
    [SerializeField] private RectTransform skillTreeParent;
    [SerializeField] GameObject groupObj;
    [SerializeField] Text MoneyText;
    public List<SkillTreeBtnObj> groups = new();
    public List<RectTransform> btn;

    TreeNode curTreeNode;

    bool isShow = false;

    [Header("SelectPanels")]
    [SerializeField] Text nameText;
    [SerializeField] Text desc;
    [SerializeField] Text cost;

    public void OnOffPanel()
    {
        isShow = !isShow;

        panels.gameObject.SetActive(isShow);
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
        //DataManager.instance.JsonSave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
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

            if (groups.Count <= count)
            {
                parnetObj = Instantiate(groupObj, skillTreeParent).GetComponent<SkillTreeBtnObj>();
                groups.Add(parnetObj);
            }
            else parnetObj = groups[count];

            if (curNode.isParent) SettingButton(curLine, parnetObj, curNode, true);

            for (int i = 0; i > curNode.childNode.Count; i++)
            {
                if (i == 0) continue;
                SettingButton(curLine, parnetObj, curNode.childNode[i]);
            }
            if (curNode.childNode.Count > 0) ReadButton(curNode.childNode[0], curLine, count + 1);
        }

        void SettingButton(int curLine, SkillTreeBtnObj parnetObj, TreeNode curNode, bool isParent = false)
        {
            Button btn;

            btn = parnetObj.AddButton(curLine, curNode, isParent);

            btn.onClick.AddListener(() =>
            {
                curTreeNode = curNode;

                SettingSelectPanel(curNode);
            });
        }

        var temp = Instantiate(groupObj, skillTreeParent).GetComponent<SkillTreeBtnObj>();
        SettingButton(1, temp, startNode, true);
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

    public void Buy()
    {
        var d = DataManager.instance;

        int cost = curTreeNode.cost;

        if (curTreeNode.parentNode != null)
        {
            if (curTreeNode.isOpen || !curTreeNode.parentNode.isOpen)
            {
                Debug.Log("해금 불가");
                return;
            }
            if (d.saveData.money < cost)
            {
                print("구매 불가 : 돈 부족");
                return;
            }
        }
        //BtnImage color Chage?

        curTreeNode.use.Use(curTreeNode.plusStats, curTreeNode.value);
        curTreeNode.isOpen = true;

        d.saveData.money -= cost;
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
    }
}
