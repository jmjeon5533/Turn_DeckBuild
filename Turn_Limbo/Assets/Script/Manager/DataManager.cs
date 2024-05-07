using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public SkillEffect[] skillEffects = new SkillEffect[2];
    public Dictionary<KeyCode, List<Skill>> skillData = new();
    public List<Skill> SkillList = new();

    public Queue<Queue<Dialogue>> curStageDialogBox = new();
    public Queue<Queue<Dialogue>> hpDialogBox = new();
    public bool isPlayer;

    public void InitUnit(Unit unit){
        if(hpDialogBox.Count == 0) return;

        Debug.Log("test");
        unit.hpLimit = hpDialogBox.Peek().Peek().hpValue;
        unit.isDialogue = false;
    }
}
