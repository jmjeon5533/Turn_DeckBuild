using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public struct RequestSkill
{
    public string skillName;
    public int minDamage;
    public int maxDamage;
    public Sprite icon;
    public Image insertImage;
    public AnimationClip animation;
}
public class Player : MonoBehaviour
{
    public Queue<RequestSkill> attackRequest = new Queue<RequestSkill>();
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
    public void AddRequest(Skill addSkill)
    {
        if (!isAttack)
        {
            print(addSkill.skillName);

            var newSkill = ConvertRequest(addSkill);
            newSkill.insertImage = UIManager.instance.AddImage(newSkill.icon);
            attackRequest.Enqueue(newSkill);
        }
    }
    public RequestSkill ConvertRequest(Skill skill)
    {
        RequestSkill newRequest = new RequestSkill();
        newRequest.animation = skill.animation;
        newRequest.minDamage = skill.minDamage;
        newRequest.maxDamage = skill.maxDamage;
        newRequest.icon = skill.icon;
        newRequest.skillName = skill.skillName;
        return newRequest;
    }

    public void UseAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator AttackMove()
    {
        UIManager.instance.cam.DOOrthoSize(3.5f,0.5f).SetEase(Ease.OutCubic);
        yield return anim.transform.DOMoveX(-2,0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
    }
    IEnumerator Attack()
    {
        yield return StartCoroutine(AttackMove());
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
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.cam.DOOrthoSize(5f,0.5f).SetEase(Ease.OutCubic);
        yield return anim.transform.DOMoveX(-3.5f,0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
        isAttack = false;
    }
}
