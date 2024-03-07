using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private bool isAttack;
    void Start()
    {
        TurnStart();
    }
    public void TurnStart()
    {
        for(int i = 0; i < enemy.skillInfo.requestCount; i++)
        AddRequest(enemy,enemy.skillInfo.skills[enemy.skillLoopCount]);
    }
    public void TurnEnd()
    {
        
    }
    void Update()
    {
        CheckInput();
        if (Input.GetKeyDown(KeyCode.A))
        {
            UseAttack();
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
    public void AddRequest(Unit target,Skill addSkill)
    {
        if (!isAttack)
        {
            print(addSkill.skillName);

            var newSkill = target.ConvertRequest(addSkill);
            newSkill.insertImage = UIManager.instance.AddImage(newSkill.icon,target.requestUIParent);
            target.attackRequest.Enqueue(newSkill);
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
                AddRequest(player,input[0]);
                SwapSkills(input);
                UIManager.instance.NextImage(i, input[0].icon);
            }
        }
    }
    public void UseAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator FirstAttackMove(Unit unit)
    {
        Vector3 movePos = Vector3.Lerp(unit.transform.position, unit.target.transform.position, 0.5f);

        yield return unit.transform.DOMoveX(movePos.x - (2 * (unit.isLeft ? 1 : -1)), 0.5f)
        .SetEase(Ease.OutCubic).WaitForCompletion();
    }
    IEnumerator Attack()
    {
        UIManager.instance.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        StartCoroutine(FirstAttackMove(player));
        yield return StartCoroutine(FirstAttackMove(enemy));
        isAttack = true;
        var attackCount = Mathf.Max(player.attackRequest.Count, enemy.attackRequest.Count);
        for (int i = 0; i < attackCount; i++)
        {
            // yield return new WaitForSeconds(skill.animation.length + 0.1f);
            float waitTime = Mathf.Max(AttackAction(player), AttackAction(enemy));
            yield return new WaitForSeconds(waitTime);
        }
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);

        player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        yield return enemy.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        isAttack = false;
    }
    float AttackAction(Unit unit)
    {
        if(unit.attackRequest.Count <= 0) return 0;
        var skill = unit.attackRequest.Dequeue();
        unit.InitCurSkillDamage(skill);
        unit.anim.Play(skill.animation.name);
        unit.iconAnim = DOTween.Sequence();
        unit.iconAnim.Append(skill.insertImage.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutQuint));
        unit.iconAnim.Append(skill.insertImage.transform.DOScale(0, 0.3f).SetEase(Ease.OutQuint));
        unit.iconAnim.AppendCallback(() =>
        {
            Destroy(skill.insertImage.gameObject);
            print("destroy");
        });
        unit.iconAnim.Play();
        return skill.animation.length;
    }

    public void SwapSkills(List<Skill> key)
    {
        var useSkills = key[0];
        key.RemoveAt(0);
        key.Add(useSkills);
    }
}
