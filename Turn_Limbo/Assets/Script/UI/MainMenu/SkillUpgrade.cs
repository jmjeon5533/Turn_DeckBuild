using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour, IInitObserver
{
    [SerializeField] Transform panels;
    [SerializeField] private RectTransform skillUpgradeBtnParent;
    [SerializeField] private Button skillUpgradeBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    [SerializeField] Text MoneyText;
    public SkillEffect playerSkills;
    public List<Button> btnImage;

    public SkillDeckBuild skillDeckBuild;
    [SerializeField] private int selectIndex;
    [SerializeField] private int selectLevel;
    private bool isShow = false;

    public int Priority => 0;

    public void Init()
    {
        ReadSpreadSheet.instance.Load(AddSkillUpgradeBtn);
    }

    public void OnOffPanel()
    {
        isShow = !isShow;

        panels.gameObject.SetActive(isShow);
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
    }

    public void AddSkillUpgradeBtn()
    {
        for (int i = skillDeckBuild.btnImage.Count - 1; i >= 0; i--)
        {
            print(i);
            Destroy(skillDeckBuild.btnImage[i].btn.gameObject);
            skillDeckBuild.btnImage.RemoveAt(i);
        }
        skillDeckBuild.AddSkillSelectBtn();
        var d = DataManager.instance;
        for (int i = 0; i < d.loadData.SkillList.Count; i++)
        {
            var btn = Instantiate(skillUpgradeBaseBtn, skillUpgradeBtnParent);
            var num = i;
            btn.onClick.AddListener(() =>
            {
                selectIndex = num;

                var skill = d.loadData.SkillList[selectIndex];

                selectLevel = playerSkills.holdSkills.ContainsKey(selectIndex) ? playerSkills.holdSkills[selectIndex].level : 0;
                explainPanel.ExplainSet(skill, selectLevel);
            });
            btn.gameObject.name = d.loadData.SkillList[i].skillName;
            btnImage.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[i].icon;
        }
        InitSkillSelectState();
        skillDeckBuild.playerSkills = playerSkills;
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++)
        {
            Color color = Color.gray;
            if (playerSkills.holdSkills.TryGetValue(i, out var holdskill))
            {
                switch (holdskill.level)
                {
                    case 0: color = Color.yellow; break;
                    case 1: color = new Color(1, 0.5f, 1, 1); break;
                    case 2: color = Color.red; break;
                    case 3: color = new Color(0.3f, 0, 1, 1); break;
                }
            }
            btnImage[i].image.color = color;
        }
        {
            if (playerSkills.holdSkills.TryGetValue(selectIndex, out var holdskill))
            {
                explainPanel.ExplainSet(DataManager.instance.loadData.SkillList[holdskill.holdIndex], holdskill.level);
            }
        }
        MoneyText.text = $"보유자원 : {DataManager.instance.saveData.money}";
        DataManager.instance.JsonSave();
    }
    public void TriggerBuySkills()
    {
        var d = DataManager.instance;
        var skill = d.loadData.SkillList[selectIndex];

        var cost = skill.sale * (selectLevel + 1);

        if (d.saveData.money < cost)
        {
            print("구매 불가 : 돈 부족");
            return;
        }

        if (playerSkills.holdSkills.TryGetValue(selectIndex, out var holdSkill))
        {
            explainPanel.ExplainSet(skill, holdSkill.level);

            var level = holdSkill.level;
            if (level >= 3)
            {
                print("구매 불가 : 최대치 도달");
                return;
            }
            holdSkill.level++;
            d.saveData.money -= cost;
            InitSkillSelectState();

            return;
        }
        HoldSkills newSkills = new HoldSkills()
        {
            holdIndex = selectIndex,
            level = 0
        };
        playerSkills.holdSkills.Add(selectIndex, newSkills);
        d.saveData.money -= skill.sale;
        explainPanel.ExplainSet(skill, newSkills.level);

        InitSkillSelectState();
        for (int i = skillDeckBuild.btnImage.Count - 1; i >= 0; i--)
        {
            //print(i);
            Destroy(skillDeckBuild.btnImage[i].btn.gameObject);
            skillDeckBuild.btnImage.RemoveAt(i);
        }
        skillDeckBuild.AddSkillSelectBtn();
    }
}
