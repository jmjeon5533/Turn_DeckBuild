using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class PanelMovement : MonoBehaviour
{
    [SerializeField] SkillEffect playerSkill;
    bool isStage = false;
    bool isMove = false;
    [SerializeField] Image warningImage;
    [SerializeField] Text warningText;
    private void Update()
    {
        if (warningImage.color.a > 0 || warningText.color.a > 0)
        {
            var value = Mathf.MoveTowards(warningImage.color.a, 0, Time.deltaTime * 0.75f);
            warningImage.color = new Color(1,1,1,value);
            warningText.color = new Color(0,0,0,value);
        }
    }
    private void WarningSkillUnSelect()
    {
        var value = 0.8f;
        warningImage.color = new Color(1,1,1,value);
        warningText.text = "스킬이 부족한 것 같아...";
        warningText.color = new Color(0,0,0,value);
    }
    public void WarningContinue()
    {
        var value = 0.8f;
        warningImage.color = new Color(1,1,1,value);
        warningText.text = "개발 중인 기능입니다...";
        warningText.color = new Color(0,0,0,value);
    }
    public void PanelMove()
    {
        if (isMove) return;
        if (isStage)
        {
            var d = DataManager.instance;
            int[] keys = new int[3];
            for (int i = 0; i < playerSkill.SelectIndex.Count; i++)
            {
                keys[d.loadData.SkillList[playerSkill.SelectIndex[i]].keyIndex]++;
            }

            if (keys.Any(n => n <= 1))
            {
                print("스킬 미할당");
                WarningSkillUnSelect();
                return;
            }
        }
        isMove = true;
        isStage = !isStage;
        transform.DOMoveX((isStage ? 1 : 0) * 1920, 0.5f).OnComplete(() => isMove = false);
    }
}
