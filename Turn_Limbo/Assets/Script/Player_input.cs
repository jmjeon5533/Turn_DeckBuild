using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int minDamage;
    public int maxDamage;
    public Sprite icon;
    public AnimationClip animation;
}

public class Player_input : MonoBehaviour
{
    private Player player;
    public Dictionary<KeyCode, List<Skill>> inputs = new();
    private static readonly KeyCode[] KEY_CODES = { KeyCode.Q, KeyCode.W, KeyCode.E };
    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        CheckInput();
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.UseAttack();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            print(player.attackRequest.Count);
        }
    }
    public void InitBtn()
    {
        for (int i = 0; i < KEY_CODES.Length; i++)
        {
            UIManager.instance.NextImage(i, inputs[KEY_CODES[i]][0].icon);
        }
    }

    private void CheckInput()
    {
        for (int i = 0; i < KEY_CODES.Length; i++)
        {
            KeyCode keyCode = KEY_CODES[i];
            if (Input.GetKeyDown(keyCode))
            {
                var input = inputs[keyCode];
                player.AddRequest(input[0]);
                SwapSkills(input);
                UIManager.instance.NextImage(i, input[0].icon);
            }
        }
    }

    public void SwapSkills(List<Skill> key)
    {
        var useSkills = key[0];
        key.RemoveAt(0);
        key.Add(useSkills);
    }
}
