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
    [SerializeField] private Transform panels;
    [SerializeField] private RectTransform[] skillViewEnterBtnParent;
    [SerializeField] private GameObject[] skillViewPanel;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Button skillSelectBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public SkillEffect playerSkills;
    public List<DeckBuildBtns> selectBtnImage;
    public List<DeckBuildBtns> viewBtnImage;
    private bool isShow = false;
    private int selectIndex = -1;
    private int selectKeyIndex = -1;
    private DeckBuildBtns selectBtn;
    public Skill IndexToSkill(int index)
    {
        for (int i = 0; i < playerSkills.selectIndex.Count; i++)
        {
            if (playerSkills.selectIndex[i] == index) return DataManager.instance.loadData.SkillList[playerSkills.selectIndex[i]];
        }
        print($"skill index {index} is Null");
        return null;
    }
    public void EnterKeyPanel(int index)
    {
        skillViewPanel[index].SetActive(true);
        for (int i = selectBtnImage.Count - 1; i >= 0; i--)
        {
            Destroy(selectBtnImage[i].btn.gameObject);
            selectBtnImage.RemoveAt(i);
        }
        AddSkillSelectBtn();
    }
    private void AddSkillViewBtn()
    {
        int[] keyCount = { 0, 0, 0 };
        for (int i = viewBtnImage.Count - 1; i >= 0; i--)
        {
            Destroy(viewBtnImage[i].btn.gameObject);
            viewBtnImage.RemoveAt(i);
        }
        var d = DataManager.instance;
        for (int i = 0; i < playerSkills.selectIndex.Count; i++)
        {
            int skills = playerSkills.selectIndex[i];
            var keyIndex = d.loadData.SkillList[skills].keyIndex;
            keyCount[keyIndex]++;
            var btn = Instantiate(skillSelectBaseBtn, skillViewEnterBtnParent[keyIndex]);

            var num = skills;
            btn.onClick.AddListener(() => 
            {
                if(selectIndex == -1)
                {
                    selectIndex = skills;
                    selectKeyIndex = keyIndex;
                    selectBtn = viewBtnImage.Find((x) => x.skillIndex == skills);

                    selectBtn.btn.image.color = Color.green;
                    print(skills);
                }
                else
                {
                    if(selectKeyIndex == keyIndex)
                    {
                        var firstIndex = d.player.selectIndex.FindIndex((x) => x == skills);
                        var secontIndex = d.player.selectIndex.FindIndex((x) => x == selectIndex);

                        var temp = d.player.selectIndex[firstIndex];
                        d.player.selectIndex[firstIndex] = d.player.selectIndex[secontIndex];
                        d.player.selectIndex[secontIndex] = temp;

                        selectBtn.btn.image.color = Color.white;
                        selectBtn = null;
                        selectIndex = -1;
                        selectKeyIndex = -1;
                        
                        AddSkillViewBtn();
                    }
                    else
                    {
                        selectIndex = skills;
                        selectKeyIndex = keyIndex;

                        selectBtn.btn.image.color = Color.white;

                        selectBtn = viewBtnImage.Find((x) => x.skillIndex == skills);
                        selectBtn.btn.image.color = Color.green;
                    }
                }
                explainPanel.ExplainSet(d.loadData.SkillList[skills],d.player.holdSkills[skills].level);
            });
            DeckBuildBtns newBtn = new DeckBuildBtns();
            newBtn.skillIndex = skills;
            newBtn.btn = btn;
            viewBtnImage.Add(newBtn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[skills].icon;
            btn.transform.GetChild(1).GetComponent<Text>().text = keyCount[keyIndex].ToString();
            btn.image.color = Color.white;
        }
    }
    public void ExitKeyPanel()
    {
        for (int i = 0; i < 3; i++) skillViewPanel[i].SetActive(false);
        AddSkillViewBtn();
    }
    public void OnOffPanel()
    {
        isShow = !isShow;
        panels.gameObject.SetActive(isShow);
        DataManager.instance.JsonSave();

        AddSkillViewBtn();
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
            selectBtnImage.Add(newBtn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.loadData.SkillList[skills.Value.holdIndex].icon;
        }

        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < selectBtnImage.Count; i++) selectBtnImage[i].btn.image.color = Color.gray;
        for (int i = 0; i < selectBtnImage.Count; i++)
        {
            if (playerSkills.selectIndex.Contains(selectBtnImage[i].skillIndex))
                selectBtnImage[i].btn.image.color = Color.yellow;
        }
    }
    public void TriggerAddSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.loadData.SkillList[index];

        if (!playerSkills.holdSkills.TryGetValue(index, out var holdSkill))
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
