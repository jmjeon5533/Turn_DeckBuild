using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDeckBuild : MonoBehaviour
{
    [SerializeField] Transform panels;
    [SerializeField] private SkillEffect playerSkill;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Button skillSelectBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public List<Button> btnImage;
    public SkillEffect playerSkills;
    bool isShow = false;
    private void Start()
    {

    }
    public Skill IndexToSkill(int index)
    {
        for (int i = 0; i < playerSkills.SelectIndex.Count; i++)
        {
            if (playerSkills.SelectIndex[i] == index) return DataManager.instance.loadData.SkillList[playerSkills.SelectIndex[i]];
        }
        print($"skill index {index} is Null");
        return null;
    }
    public void OnOffPanel()
    {
        isShow = !isShow;
        for (int i = btnImage.Count - 1; i >= 0; i--)
        {
            print(i);
            Destroy(btnImage[i].gameObject);
            btnImage.RemoveAt(i);
        }
        panels.gameObject.SetActive(isShow);

        AddSkillSelectBtn();
    }
    public void AddSkillSelectBtn()
    {
        var d = DataManager.instance;
        for (int i = 0; i < playerSkill.holdSkills.Count; i++)
        {
            var btn = Instantiate(skillSelectBaseBtn, skillSelectBtnParent[d.loadData.SkillList[playerSkill.holdSkills[i].holdIndex].keyIndex]);
            var num = i;
            btn.onClick.AddListener(() => TriggerAddSkills(num));
            btnImage.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[playerSkill.holdSkills[i].holdIndex].icon;
        }
        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].image.color = Color.gray;
        for (int i = 0; i < playerSkills.SelectIndex.Count; i++)
        {
            btnImage[playerSkills.SelectIndex[i]].image.color = Color.yellow;
        }
    }
    public void TriggerAddSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.loadData.SkillList[index];
        for (int i = 0; i < playerSkills.SelectIndex.Count; i++)
        {
            if (playerSkills.SelectIndex[i] == index)
            {
                for (int j = 0; j < playerSkill.holdSkills.Count; j++)
                {
                    if (playerSkill.holdSkills[j].holdIndex == index) explainPanel.ExplainSet(skill, playerSkill.holdSkills[j].level);
                }

                playerSkills.SelectIndex.RemoveAt(i);
                InitSkillSelectState();

                return;
            }
        }
        for (int j = 0; j < playerSkill.holdSkills.Count; j++)
        {
            if (playerSkill.holdSkills[j].holdIndex == index) explainPanel.ExplainSet(skill, playerSkill.holdSkills[j].level);
        }
        playerSkills.SelectIndex.Add(index);
        InitSkillSelectState();
    }
}
