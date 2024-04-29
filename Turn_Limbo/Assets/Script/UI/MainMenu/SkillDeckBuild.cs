using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDeckBuild : MonoBehaviour
{
    [SerializeField] private SkillEffect playerSkill;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Button skillSelectBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public List<Button> btnImage;
    public SkillEffect playerSkills;
    bool isShow = false;
    private void Start()
    {
        ReadSpreadSheet.instance.Load(AddSkillSelectBtn);
    }
    public void OnOffPanel()
    {
        isShow = !isShow;
        gameObject.SetActive(isShow);
    }
    private void AddSkillSelectBtn()
    {
        var d = DataManager.instance;
        for (int i = 0; i < d.SkillList.Count; i++)
        {
            var btn = Instantiate(skillSelectBaseBtn, skillSelectBtnParent[d.SkillList[i].keyIndex]);
            var num = i;
            btn.onClick.AddListener(() => TriggerAddSkills(num));
            btnImage.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.SkillList[i].icon;
        }
        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].image.color = Color.gray;
        for (int i = 0; i < playerSkills.holdIndex.Count; i++)
        {
            btnImage[playerSkills.holdIndex[i]].image.color = Color.yellow;
        }
    }
    public void TriggerAddSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.SkillList[index];
        for (int i = 0; i < playerSkills.holdIndex.Count; i++)
        {
            if (playerSkills.holdIndex[i] == index)
            {
                explainPanel.ExplainSet(skill.icon, skill.skillName, skill.explain);
                playerSkills.holdIndex.RemoveAt(i);
                InitSkillSelectState();

                return;
            }
        }
        explainPanel.ExplainSet(skill.icon, skill.skillName, skill.explain);
        playerSkills.holdIndex.Add(index);
        InitSkillSelectState();
    }
}
