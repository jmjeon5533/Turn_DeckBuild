using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int minDamage;
    public int maxDamage;
    public Sprite icon;
    public AnimationClip animation;
}

public class Controller : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    void Start()
    {

    }
    void Update()
    {
        CheckInput();
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.UseAttack();
            enemy.UseAttack();
        }
    }
        public Dictionary<KeyCode, List<Skill>> inputs = new();
    private static readonly KeyCode[] KEY_CODES = { KeyCode.Q, KeyCode.W, KeyCode.E };
        
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
