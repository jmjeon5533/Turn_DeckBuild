using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeckBuildBtns
{
    public int skillIndex;
    public Button btn;
}
public class SkillDeckBuild : MonoBehaviour
{
    [SerializeField] Transform panels;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Button skillSelectBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public SkillEffect playerSkills;
    public List<DeckBuildBtns> btnImage;
    bool isShow = false;
    public Skill IndexToSkill(int index)
    {
        for (int i = 0; i < playerSkills.selectIndex.Count; i++)
        {
            if (playerSkills.selectIndex[i] == index) return DataManager.instance.loadData.SkillList[playerSkills.selectIndex[i]];
        }
        print($"skill index {index} is Null");
        return null;
    }
    public void OnOffPanel()
    {
        isShow = !isShow;
        for (int i = btnImage.Count - 1; i >= 0; i--)
        {
            Destroy(btnImage[i].btn.gameObject);
            btnImage.RemoveAt(i);
        }
        panels.gameObject.SetActive(isShow);
        DataManager.instance.JsonSave();

        AddSkillSelectBtn();
    }
    public void AddSkillSelectBtn()
    {
        var d = DataManager.instance;
        foreach (var skills in playerSkills.holdSkills)
        {
            var btn = Instantiate(skillSelectBaseBtn, skillSelectBtnParent[d.loadData.SkillList[skills.Value.holdIndex].keyIndex]);
            var num = skills.Key;
            btn.onClick.AddListener(() => TriggerAddSkills(num));
            DeckBuildBtns newBtn = new DeckBuildBtns();
            newBtn.skillIndex = skills.Value.holdIndex;
            newBtn.btn = btn;
            btnImage.Add(newBtn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[skills.Value.holdIndex].icon;
        }
        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].btn.image.color = Color.gray;
        for (int i = 0; i < btnImage.Count; i++)
        {
            if (playerSkills.selectIndex.Contains(btnImage[i].skillIndex))
                btnImage[i].btn.image.color = Color.yellow;
        }
    }
    public void TriggerAddSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.loadData.SkillList[index];

        if(!playerSkills.holdSkills.TryGetValue(index, out var holdSkill))
            return;

        for (int i = 0; i < playerSkills.selectIndex.Count; i++)
        {
            if (playerSkills.selectIndex[i] == index)
            {
                    explainPanel.ExplainSet(skill, holdSkill.level);

                playerSkills.selectIndex.RemoveAt(i);
                InitSkillSelectState();

                return;
            }
        }
        explainPanel.ExplainSet(skill, holdSkill.level);
        playerSkills.selectIndex.Add(holdSkill.holdIndex);
        InitSkillSelectState();
    }
}
