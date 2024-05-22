using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageStartBtn : MonoBehaviour
{
    [SerializeField] SkillEffect playerSkill;
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var num = i;
            var btn = transform.GetChild(num).GetComponent<Button>();
            btn.onClick.AddListener(() => StageStart(num));
        }   
    }
    public void StageStart(int stageIndex)
    {
        var d = DataManager.instance;
        int[] keys = new int[3];
        for(int i = 0; i < playerSkill.SelectIndex.Count; i++)
        {
            keys[d.SkillList[playerSkill.SelectIndex[i]].keyIndex]++;
        }

        if(keys.Any(n => n <= 1))
        {
            print("스킬 미할당");
            return;
        }
        //Temporary index
        ReadSpreadSheet.instance.curStageID = 2;
        SceneManager.LoadScene(2);
    }
}
