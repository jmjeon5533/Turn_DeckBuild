using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectBtn
{
    public List<Image> btns;
}
public class SkillDeckBuild : MonoBehaviour
{
    [SerializeField] private SkillEffect playerSkill;
    [SerializeField] private RectTransform[] skillSelectBtnParent;
    [SerializeField] private Image skillSelectBaseBtn;
    public SelectBtn[] BtnImage;
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
            BtnImage[d.SkillList[i].keyIndex].btns.Add(btn);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = d.SkillList[i].icon;
        }
    }
    
}
