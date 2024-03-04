using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public struct Requestskill
{
    public string skillName;
    public int attackDamage;
    public Sprite icon;
    public Image insertImage;
    public AnimationClip animation;
}
public class Player : MonoBehaviour
{
    public Queue<Requestskill> attackRequest = new Queue<Requestskill>();
    Animator anim;
    bool isAttack;
    Sequence iconAnim;

    private void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }
    private void Start()
    {

    }
    public void AddRequest(skill addSkill)
    {
        if (!isAttack)
        {
            print(addSkill.skillName);

            var newSkill = ConvertRequest(addSkill);
            newSkill.insertImage = UIManager.instance.AddImage(newSkill.icon);
            attackRequest.Enqueue(newSkill);
        }
    }
    public Requestskill ConvertRequest(skill skill)
    {
        Requestskill newRequest = new Requestskill();
        newRequest.animation = skill.animation;
        newRequest.attackDamage = skill.attackDamage;
        newRequest.icon = skill.icon;
        newRequest.skillName = skill.skillName;
        return newRequest;
    }

    public void UseAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        isAttack = true;
        var count = attackRequest.Count;
        for (int i = 0; i < count; i++)
        {
            var skill = attackRequest.Dequeue();
            print($"Attacked : {skill.skillName}");
            anim.Play(skill.animation.name);
            iconAnim = DOTween.Sequence();
            iconAnim.Append(skill.insertImage.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutQuint));
            iconAnim.Append(skill.insertImage.transform.DOScale(0, 0.3f).SetEase(Ease.OutQuint));
            iconAnim.AppendCallback(() =>
            {
                Destroy(skill.insertImage.gameObject);
                print("destroy");
            });
            yield return new WaitForSeconds(skill.animation.length + 0.1f);
            iconAnim.Play();
        }
        isAttack = false;
    }
}
