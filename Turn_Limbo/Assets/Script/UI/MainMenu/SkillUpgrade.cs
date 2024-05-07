using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour
{
    [SerializeField] private RectTransform skillUpgradeBtnParent;
    [SerializeField] private Button skillUpgradeBaseBtn;
    [SerializeField] SkillExplain explainPanel;
    public List<Button> btnImage;
    bool isShow = false;
    public void OnOffPanel()
    {
        isShow = !isShow;
        gameObject.SetActive(isShow);
    }
    private void Start()
    {
        //ReadSpreadSheet.instance.Load(AddSkillUpgradeBtn);
    }
    private void AddSkillUpgradeBtn()
    {
        var d = DataManager.instance;
        for (int i = 0; i < d.SkillList.Count; i++)
        {
            var btn = Instantiate(skillUpgradeBaseBtn, skillUpgradeBtnParent);
            var num = i;
            btn.onClick.AddListener(() => TriggerSelectSkills(num));
            btnImage.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.SkillList[i].icon;
        }
        InitSkillSelectState();
    }
    private void InitSkillSelectState()
    {
        for (int i = 0; i < btnImage.Count; i++) btnImage[i].image.color = Color.gray;
    }
    public void TriggerSelectSkills(int index)
    {
        var d = DataManager.instance;
        var skill = d.SkillList[index];

        explainPanel.ExplainSet(skill.icon, skill.skillName, skill.explain);
        InitSkillSelectState();
    }
}
