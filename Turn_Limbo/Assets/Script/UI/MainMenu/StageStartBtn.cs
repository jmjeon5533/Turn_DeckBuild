using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageStartBtn : MonoBehaviour, IInitObserver
{
    public int Priority => 1;

    public void Init()
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
        DataManager.instance.curMode = Controller.Modes.stage;
        DataManager.instance.curStageID = stageIndex;
        SceneManager.LoadScene(2);
    }
    public void RogLikeStart()
    {
        DataManager.instance.curMode = Controller.Modes.rogLike;
        DataManager.instance.curStageID = -1;
        SceneManager.LoadScene(2);
    }
}
