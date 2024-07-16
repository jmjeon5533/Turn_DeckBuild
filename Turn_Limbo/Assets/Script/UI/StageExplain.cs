using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageExplain : MonoBehaviour
{
    Image icon;
    [SerializeField] TMP_Text getMoneyText;
    [SerializeField] Text stageNameText;
    [SerializeField] TMP_Text spawnEnemyListText;
    [SerializeField] Scrollbar horizontalBar;
    [SerializeField] Transform imgSetParent;
    private void Awake()
    {
        icon = GetComponent<Image>();
    }
    public void SetExplain(RogStageData stageData, Controller controller)
    {
        icon.sprite = stageData.icon;
        getMoneyText.text = $"<size=80>{stageData.clearGetMoney}</size>xp";
        stageNameText.text = stageData.stageName;
        List<string> enemyNames = new List<string>();
        List<int> enemyCounts = new List<int>();
        for(int i = 0; i < stageData.spawnEnemyList.Length; i++)
        {
            var name = stageData.spawnEnemyList[i].unitName;
            if(enemyNames.Contains(name))
            {
                var index = enemyNames.FindIndex((x) => x == name);
                enemyCounts[index]++;
            }
            else
            {
                enemyNames.Add(name);
                enemyCounts.Add(1);
            }
        }
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < enemyNames.Count; i++)
        {
            sb.Append($"<align=\"flush\"><color=black>{enemyNames[i]}</color>    x{enemyCounts[i]}");
            if(i < enemyNames.Count - 1) sb.Append("\n");
        }
        spawnEnemyListText.text = sb.ToString();
        for(int i = 0; i < stageData.getSkillIndex.Length; i++)
        {
            print($"{DataManager.instance.loadData.SkillList[stageData.getSkillIndex[i]].index}, {stageData.getSkillIndex[i]}");
            if(controller.player.skillInfo.holdSkills.ContainsKey(stageData.getSkillIndex[i])) return;

            var skills = Instantiate(new GameObject(),imgSetParent).AddComponent<Image>();
            skills.sprite = DataManager.instance.loadData.SkillList[stageData.getSkillIndex[i]].icon;
        }
        StartCoroutine(Scroll());
    }
    IEnumerator Scroll()
    {
        horizontalBar.value = 0;
        yield return new WaitForSecondsRealtime(0.3f);
        for(float f = 0; f < 1f; f += Time.unscaledDeltaTime / 3)
        {
            horizontalBar.value = f;
            yield return null;
        }
        if(RoglikeManager.instance.isEvent) StartCoroutine(Scroll());
    }
}
