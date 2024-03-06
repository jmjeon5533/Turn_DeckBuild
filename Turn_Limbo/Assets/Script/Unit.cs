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
public class Unit : MonoBehaviour
{
    public Queue<RequestSkill> attackRequest = new Queue<RequestSkill>();
    public int hp;
    public int maxHP;

    public int shield;
    public int maxShield;

    Animator anim;
    protected bool isAttack;
    protected bool isLeft;
    Sequence iconAnim;

    public Unit target;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image shieldImage;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        isLeft = target.transform.position.x > transform.position.x;
    }
    protected virtual void Update()
    {
        hpImage.rectTransform.anchoredPosition 
        = UIManager.instance.cam.WorldToScreenPoint(transform.position + (new Vector3(-2f,0) * (isLeft ? 1 : -1)));
        hpImage.fillAmount = (float)hp / maxHP;
        shieldImage.fillAmount = (float)shield / maxShield;
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
        Vector3 movePos = Vector3.Lerp(transform.position, target.transform.position,0.5f);
        
        UIManager.instance.cam.DOOrthoSize(3.5f,0.5f).SetEase(Ease.OutCubic);
        yield return transform.DOMoveX(movePos.x - (1 * (isLeft ? 1 : -1)),0.5f)
        .SetEase(Ease.OutCubic).WaitForCompletion();
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
        yield return transform.DOMoveX(-3.5f * (isLeft ? 1 : -1),0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        isAttack = false;
    }
    public void Damage(int Damage)
    {
        hp -= Damage;
    }
}
