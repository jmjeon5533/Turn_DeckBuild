using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageStartBtn : MonoBehaviour
{
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
        //Temporary index
        Debug.Log(stageIndex);
        DataManager.instance.curStageID = stageIndex;
        SceneManager.LoadScene(2);
    }
}
