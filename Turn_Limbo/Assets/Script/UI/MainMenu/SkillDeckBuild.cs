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
    [SerializeField] private SkillEffect playerSkill;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Button skillSelectBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public List<DeckBuildBtns> btnImage;
    public SkillEffect playerSkills;
    bool isShow = false;
    private void Start()
    {

    }
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
            //print(i);
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
        for (int i = 0; i < playerSkill.holdSkills.Count; i++)
        {
            var btn = Instantiate(skillSelectBaseBtn, skillSelectBtnParent[d.loadData.SkillList[playerSkill.holdSkills[i].holdIndex].keyIndex]);
            var num = i;
            btn.onClick.AddListener(() => TriggerAddSkills(num));
            DeckBuildBtns newBtn = new DeckBuildBtns();
            newBtn.skillIndex = playerSkill.holdSkills[i].holdIndex;
            newBtn.btn = btn;
            btnImage.Add(newBtn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[playerSkill.holdSkills[i].holdIndex].icon;
        }
        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].btn.image.color = Color.gray;
        for (int i = 0; i < btnImage.Count; i++)
        {
            if(playerSkills.selectIndex.Contains(btnImage[i].skillIndex))
            btnImage[i].btn.image.color = Color.yellow;
        }
    }
    public void TriggerAddSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.loadData.SkillList[btnImage[index].skillIndex];
        for (int i = 0; i < playerSkills.selectIndex.Count; i++)
        {
            if (playerSkills.selectIndex[i] == btnImage[index].skillIndex)
            {
                for (int j = 0; j < playerSkill.holdSkills.Count; j++)
                {
                    if (playerSkill.holdSkills[j].holdIndex == btnImage[index].skillIndex) explainPanel.ExplainSet(skill, playerSkill.holdSkills[j].level);
                }

                playerSkills.selectIndex.RemoveAt(i);
                InitSkillSelectState();

                return;
            }
        }
        for (int j = 0; j < playerSkill.holdSkills.Count; j++)
        {
            if (playerSkill.holdSkills[j].holdIndex == btnImage[index].skillIndex) explainPanel.ExplainSet(skill, playerSkill.holdSkills[j].level);
        }
        playerSkills.selectIndex.Add(playerSkill.holdSkills[index].holdIndex);
        InitSkillSelectState();
    }
}
