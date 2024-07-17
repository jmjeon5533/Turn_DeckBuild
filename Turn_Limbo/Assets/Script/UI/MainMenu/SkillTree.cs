using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TreeNode
{
    public TreeNode parentNode;
    public List<TreeNode> childNode = new();

    public string name;
    public string desc;
    public SkillTreeScript use;
    public string value;
    public int cost;

    public int lineNum;
    public bool isParent;
    public bool isOpen;

    public PlusStats plusStats;
    public Sprite sprite;
    public Button btn;

    public SaveTreeData SaveData()
    {
        List<bool> _lineOne = new();
        List<bool> _lineTwo = new();
        List<bool> _lineThree = new();

        List<bool> _childLineOne = new();
        List<bool> _childLineTwo = new();
        List<bool> _childLineThree = new();

        GetData(this);
        SaveTreeData data = new()
        {
            startNode = isOpen,
        };
        data.parent.Add(_lineOne);
        data.parent.Add(_lineTwo);
        data.parent.Add(_lineThree);

        data.child.Add(_childLineOne);
        data.child.Add(_childLineTwo);
        data.child.Add(_childLineThree);

        return data;

        void GetData(TreeNode curNode)
        {
            if (curNode.childNode.Count == 0) return;

            foreach (var n in curNode.childNode)
                if (n.isParent) switch (n.lineNum)
                    {
                        case 1: _lineOne.Add(n.isOpen); break;
                        case 2: _lineTwo.Add(n.isOpen); break;
                        case 3: _lineThree.Add(n.isOpen); break;
                    }
                else switch (n.lineNum)
                    {
                        case 1: _childLineOne.Add(n.isOpen); break;
                        case 2: _childLineTwo.Add(n.isOpen); break;
                        case 3: _childLineThree.Add(n.isOpen); break;
                    }

            for (int i = 0; i < curNode.childNode.Count; i++) GetData(curNode.childNode[i]);
        }
    }
}

[Serializable]
public class SaveTreeData
{
    public bool startNode;
    public List<List<bool>> parent = new();
    public List<List<bool>> child = new();
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
        DataManager.instance.JsonSave();
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

            for (int i = 0; i < curNode.childNode.Count; i++)
            {
                if (i == 0) continue;
                SettingButton(curLine, parnetObj, curNode.childNode[i]);
            }
            if (curNode.childNode.Count > 0) ReadButton(curNode.childNode[0], curLine, count + 1);
        }

        void SettingButton(int curLine, SkillTreeBtnObj parnetObj, TreeNode curNode, bool isParent = false)
        {
            Button btn;

            btn = parnetObj.AddButton(curLine, isParent);
            btn.GetComponent<Image>().sprite = curNode.sprite;
            if (curNode.isOpen)
            {
                curNode.use.Use(curNode.plusStats, curNode.value);
                btn.image.color = new Color(1, 1, 1);
            }

            btn.onClick.AddListener(() =>
            {
                curTreeNode = curNode;

                nameText.text = curNode.name;
                desc.text = curNode.desc;
                cost.text = curNode.cost.ToString();
            });
            curNode.btn = btn;
        }

        var temp = Instantiate(groupObj, skillTreeParent).GetComponent<SkillTreeBtnObj>();
        SettingButton(1, temp, startNode, true);
        for (int i = 0; i < 3; i++)
        {
            ReadButton(startNode.childNode[i], i, 0);
        }
    }

    public void Buy()
    {
        var d = DataManager.instance;

        int cost = curTreeNode.cost;

        if (curTreeNode.isOpen || d.saveData.money < cost || !curTreeNode.parentNode.isOpen)
        {
            Debug.Log($"해금 불가 - {curTreeNode.name}");
            if (!curTreeNode.isOpen)
            {
                curTreeNode.btn.image.color = new Color(1, 0, 0);
                curTreeNode.btn.image.DOColor(new Color(0.7f, 0.7f, 0.7f), 0.5f);
            }
            return;
        }

        Debug.Log($"해금 - {curTreeNode.name}");
        curTreeNode.use.Use(curTreeNode.plusStats, curTreeNode.value);
        curTreeNode.isOpen = true;
        curTreeNode.btn.image.color = new Color(1, 1, 1);

        d.saveData.money -= cost;
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
        DataManager.instance.JsonSave();
    }

    public void ReSetSkillTree()
    {
        var d = DataManager.instance;
        TreeNode startNode = d.startNode;

        void Init(TreeNode curNode)
        {
            foreach (var n in curNode.childNode)
            {
                //Debug.Log($"Check : {n.desc}");
                if (n.isOpen)
                {
                    //Debug.Log($"Close : {n.desc}");
                    n.isOpen = false;
                    d.saveData.money += n.cost;
                    n.btn.image.color = new Color(0.7f, 0.7f, 0.7f);
                }
            }
            foreach (var n in curNode.childNode) Init(n);
        }

        if (startNode.isOpen)
        {
            startNode.isOpen = false;
            d.saveData.money += startNode.cost;
            startNode.btn.image.color = new Color(0.7f, 0.7f, 0.7f);
        }
        Init(startNode);

        d.plusStats.Init();
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
        DataManager.instance.JsonSave();
    }
}
