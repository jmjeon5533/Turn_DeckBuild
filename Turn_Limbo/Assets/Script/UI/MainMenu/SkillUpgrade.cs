using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour
{
    [SerializeField] Transform panels;
    [SerializeField] private RectTransform skillUpgradeBtnParent;
    [SerializeField] private Button skillUpgradeBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    [SerializeField] Text MoneyText;
    public SkillEffect playerSkills;
    public List<Button> btnImage;

    public SkillDeckBuild skillDeckBuild;
    private int selectIndex;
    private bool isShow = false;
    private void Start()
    {
        ReadSpreadSheet.instance.Load(AddSkillUpgradeBtn);
    }
    public void OnOffPanel()
    {
        isShow = !isShow;

        panels.gameObject.SetActive(isShow);
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.Money}";
    }
    
    public void AddSkillUpgradeBtn()
    {
        for (int i = skillDeckBuild.btnImage.Count - 1; i >= 0; i--)
        {
            print(i);
            Destroy(skillDeckBuild.btnImage[i].gameObject);
            skillDeckBuild.btnImage.RemoveAt(i);
        }
        skillDeckBuild.AddSkillSelectBtn();
        var d = DataManager.instance;
        for (int i = 0; i < d.SkillList.Count; i++)
        {
            var btn = Instantiate(skillUpgradeBaseBtn, skillUpgradeBtnParent);
            var num = i;
            btn.onClick.AddListener(() =>
            {
                selectIndex = num;

                var skill = d.SkillList[selectIndex];

                int level = 0;
                for (int j = 0; j < playerSkills.holdSkills.Count; j++)
                {
                    if (playerSkills.holdSkills[j].holdIndex == selectIndex) level = playerSkills.holdSkills[j].level;
                }
                explainPanel.ExplainSet(skill, level);
            });
            btn.gameObject.name = d.SkillList[i].skillName;
            btnImage.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.SkillList[i].icon;
        }
        InitSkillSelectState();
        skillDeckBuild.playerSkills = playerSkills;
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].image.color = Color.gray;
        for (int i = 0; i < playerSkills.holdSkills.Count; i++)
        {
            Color color = Color.gray;
            switch (playerSkills.holdSkills[i].level)
            {
                case 0: color = Color.yellow; break;
                case 1: color = new Color(1, 0.5f, 1, 1); break;
                case 2: color = Color.red; break;
            }
            btnImage[playerSkills.holdSkills[i].holdIndex].image.color = color;
        }
        for (int j = 0; j < playerSkills.holdSkills.Count; j++)
        {
            if (playerSkills.holdSkills[j].holdIndex == selectIndex) 
            explainPanel.ExplainSet(DataManager.instance.SkillList[playerSkills.holdSkills[j].holdIndex], playerSkills.holdSkills[j].level);
        }
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.Money}";
    }
    public void TriggerBuySkills()
    {
        var d = DataManager.instance;
        var skill = d.SkillList[selectIndex];

        if(d.saveData.Money < 150) 
        {
            print("구매 불가 : 돈 부족");
            return;
        }

        for (int i = 0; i < playerSkills.holdSkills.Count; i++)
        {
            if (playerSkills.holdSkills[i].holdIndex == selectIndex)
            {
                for (int j = 0; j < playerSkills.holdSkills.Count; j++)
                {
                    if (playerSkills.holdSkills[j].holdIndex == selectIndex) explainPanel.ExplainSet(skill, playerSkills.holdSkills[j].level);
                }

                var level = playerSkills.holdSkills[i].level;
                if (level >= 2) 
                {
                    print("구매 불가 : 최대치 도달");
                    return;
                }
                playerSkills.holdSkills[i].level++;
                d.saveData.Money -= 150;
                InitSkillSelectState();

                return;
            }
        }
        holdSkills newSkills = new holdSkills()
        {
            holdIndex = selectIndex,
            level = 0
        };
        playerSkills.holdSkills.Add(newSkills);
        d.saveData.Money -= 150;
        for (int j = 0; j < playerSkills.holdSkills.Count; j++)
        {
            if (playerSkills.holdSkills[j].holdIndex == selectIndex) explainPanel.ExplainSet(skill, playerSkills.holdSkills[j].level);
        }
        InitSkillSelectState();
        for (int i = skillDeckBuild.btnImage.Count - 1; i >= 0; i--)
        {
            print(i);
            Destroy(skillDeckBuild.btnImage[i].gameObject);
            skillDeckBuild.btnImage.RemoveAt(i);
        }
        skillDeckBuild.AddSkillSelectBtn();
    }
}
