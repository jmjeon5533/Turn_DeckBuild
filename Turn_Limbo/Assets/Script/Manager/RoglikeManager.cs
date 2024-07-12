using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[System.Serializable]
public class RogStageData
{
    public int[] getSkillIndex;
    public Enemy[] spawnEnemyList;
}
[CreateAssetMenu(fileName = "RogData", menuName = "Roglike", order = 0)]
public class RoglikeData : ScriptableObject
{
    public List<RogStageData> stageDatas = new();
}
public class RoglikeManager : MonoBehaviour
{
    public static RoglikeManager instance {get; private set;}

    public bool isEvent;
    public SkillExplain[] skillExplains;
    private void Awake()
    {
        instance = this;
    }
    public enum Modes
    {
        stage,
        rogLike
    }
    public void TryGetItem()
    {

    }
    public void ShowGetSkillPanel()
    {
        isEvent = true;
        Time.timeScale = 0;
        StartCoroutine(ShowGetSkillAnim());
    }
    private IEnumerator ShowGetSkillAnim()
    {
        yield return StartCoroutine(UIManager.instance.UseFadePanel());
        for(int i = 0; i < skillExplains.Length; i++)
        {
            skillExplains[i].transform.DOMoveX(-500 + (i * 500), 0.5f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.1f * (i + 1));
        }
    }
}
