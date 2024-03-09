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
    public int attackCount;
    public Sprite icon;
    public SkillScript effect;
    public Image insertImage;
    public AnimationClip animation;
    public Unit.ActionType actionType;
}
public class Unit : MonoBehaviour
{
    public enum ActionType
    {
        none,
        Attack,
        Defence,
        Dodge
    }
    public enum PropertyType
    {

    }
    public Queue<RequestSkill> attackRequest = new Queue<RequestSkill>();
    public int hp;
    public int maxHP;

    public int shield;
    public int maxShield;

    public int curDamage;
    public RequestSkill curSkill;
    public readonly RequestSkill nullSkill = new RequestSkill()
    {
        actionType = ActionType.none
    };
    public ParticleSystem effect;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public Sequence iconAnim;
    public SkillEffect skillInfo;

    public RectTransform requestUIParent;

    public Unit target;
    [SerializeField] protected RectTransform statParent;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image shieldImage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        isLeft = target.transform.position.x > transform.position.x;

    }
    protected virtual void Update()
    {
        UIUpdate();
    }
    public virtual void AttackStart(RequestSkill skill)
    {
        //skill.effect.Start();
        print(target.curSkill.actionType);
    }
    public virtual void AttackEnd(RequestSkill skill)
    {
        //skill.effect.End();
        curSkill = nullSkill;
    }
    public virtual void Attacking()
    {
        int totalDmg = 0;
        print(target.curSkill.actionType);
        switch(target.curSkill.actionType)
        {
            case ActionType.none:
            {
                totalDmg = curDamage;
                target.Damage(curDamage);
            }
            break;
            case ActionType.Attack:
            {
                totalDmg = curDamage;
                target.ShieldDamage(totalDmg);
            }
            break;
            case ActionType.Defence:
            {
                totalDmg = Mathf.Clamp(curDamage - target.curDamage,0,999);
                target.Damage(totalDmg);
            }
            break;
            case ActionType.Dodge:
            {
                if(target.curDamage < curDamage)
                {
                    totalDmg = curDamage;
                    target.Damage(totalDmg);
                }
            }
            break;
        }
        UIManager.instance.DamageText(totalDmg,transform.position + Vector3.right * (isLeft ? 2.5f : -2.5f));
        //skill.effect.Attacking();
        var cam = UIManager.instance.cam;
        cam.transform.position = cam.transform.position + ((Vector3)Random.insideUnitCircle.normalized * 1);
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        Instantiate(effect,transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2),Quaternion.identity);
        cam.orthographicSize = 2;
    }
    void UIUpdate()
    {
        var ui = UIManager.instance;
        statParent.anchoredPosition 
        = ui.cam.WorldToScreenPoint(transform.position + (new Vector3(-2f,0) * (isLeft ? 1 : -1)));
        hpImage.fillAmount = (float)hp / maxHP;
        shieldImage.fillAmount = (float)shield / maxShield;

        requestUIParent.anchoredPosition 
        = ui.cam.WorldToScreenPoint(transform.position
        + new Vector3((3 - (5 - ui.cam.orthographicSize)) * (isLeft ? 1 : -1),2.5f + (5 - ui.cam.orthographicSize) * 0.5f));
        requestUIParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
    }
    public void InitCurSkillDamage(RequestSkill skill)
    {
        curDamage = Mathf.FloorToInt((float)Random.Range(skill.minDamage,skill.maxDamage + 1) / skill.attackCount);
        print($"name = {skill.skillName}, Damage = {curDamage} : SkillDmg = {skill.minDamage}, {skill.maxDamage} : count = {skill.attackCount}");
    }
    public RequestSkill ConvertRequest(Skill skill)
    {
        RequestSkill newRequest = new RequestSkill();
        newRequest.animation = skill.animation;
        newRequest.minDamage = skill.minDamage;
        newRequest.maxDamage = skill.maxDamage;
        newRequest.actionType = skill.actionType;
        newRequest.attackCount = skill.attackCount;
        newRequest.effect = skill.effect;
        newRequest.icon = skill.icon;
        newRequest.skillName = skill.skillName;
        return newRequest;
    }
    public void ShieldDamage(int damage)
    {
        if(shield <= damage)
        {
            Damage(damage - shield);
            shield = 0;
        }
        else shield -= damage;
    }
    public void Damage(int damage)
    {
        hp -= shield <= 0 ? Mathf.CeilToInt(1.5f * damage) : damage;
    }
}
