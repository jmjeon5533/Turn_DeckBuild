using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDeckBuild : MonoBehaviour
{
    [SerializeField] private SkillEffect playerSkill;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Image skillSelectBaseBtn;
    private void Start()
    {
        ReadSpreadSheet.instance.Load(InitSkillSelectBtn);
    }
    private void InitSkillSelectBtn()
    {
        var d = DataManager.instance;
        for(int i = 0 ; i < d.SkillList.Count; i++)
        {
            var btn = Instantiate(skillSelectBaseBtn,skillSelectBtnParent[d.SkillList[i].keyIndex]);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.SkillList[i].icon;
        }
    }
    
}
