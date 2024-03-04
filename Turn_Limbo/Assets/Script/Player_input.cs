using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class keyInput
{
    public KeyCode key;
    public List<skill> insertSkills = new List<skill>();
}
[System.Serializable]
public class skill
{
    public string skillName;
    public int attackDamage;
    public Sprite icon;
    public AnimationClip animation;
}
public class Player_input : MonoBehaviour
{
    private Player player;
    public List<keyInput> inputs = new List<keyInput>();
    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {
        for(int i = 0; i < inputs.Count; i++)
        {
            UIManager.instance.NextImage(i,inputs[i].insertSkills[0].icon);
        }
    }
    private void Update()
    {
        for(int i = 0; i < inputs.Count; i++)
        {
            if(Input.GetKeyDown(inputs[i].key))
            {
                player.AddRequest(inputs[i].insertSkills[0]);
                SwapSkills(inputs[i]);
                UIManager.instance.NextImage(i,inputs[i].insertSkills[0].icon);
            }
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            player.UseAttack();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            print(player.attackRequest.Count);
        }
    }
    public void SwapSkills(keyInput key)
    {
        var useSkills = key.insertSkills[0];
        key.insertSkills.RemoveAt(0);
        key.insertSkills.Add(useSkills);
    }
}
