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
public abstract class Unit : MonoBehaviour
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
    public string unitName => gameObject.name;
    public int hp;
    public int maxHP;

    public int shield;
    public int maxShield;

    public bool shieldBreak;

    public int curDamage;
    public Unit target;
    public RequestSkill curSkill;
    public readonly RequestSkill nullSkill = new RequestSkill()
    {
        actionType = ActionType.none
    };
    public ParticleSystem effect;
    public AudioClip hitSound;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public Sequence iconAnim;

    public RectTransform requestUIParent;
    [SerializeField] protected RectTransform statParent;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image hpAnimImage;
    [SerializeField] protected Image shieldImage;
    [SerializeField] protected Image shieldAnimImage;
    [HideInInspector] public float dmgDelayTime;
    [SerializeField] private float dmgDelayCurTime;

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
    public virtual void TurnInit()
    {
        if (shield <= 0 && !shieldBreak)
        {
            shieldBreak = true;
            return;
        }
        if (shieldBreak)
        {
            shield = maxShield;
            shieldBreak = false;
        }
    }
    public virtual void AttackStart(RequestSkill skill)
    {
        //skill.effect.Start();
    }
    public virtual void AttackEnd(RequestSkill skill)
    {
        //skill.effect.End();
        curSkill = nullSkill;
    }
    public virtual void Attacking()
    {
        switch (target.curSkill.actionType)
        {
            case ActionType.none:
                {
                    target.Damage(curDamage);
                }
                break;
            case ActionType.Attack:
                {
                    target.ShieldDamage(curDamage);
                }
                break;
            case ActionType.Defence:
                {
                    target.Damage(Mathf.Clamp(curDamage - target.curDamage, 0, 999));
                }
                break;
            case ActionType.Dodge:
                {
                    if (target.curDamage < curDamage)
                    {
                        target.Damage(curDamage);
                    }
                }
                break;
        }
        //skill.effect.Attacking();
        var cam = UIManager.instance.cam;
        cam.transform.position = cam.transform.position + ((Vector3)Random.insideUnitCircle.normalized * 1);
        UIManager.instance.camRotZ -=  Random.Range(UIManager.instance.camRotZ/2,UIManager.instance.camRotZ*2.5f);
        SoundManager.instance.SetAudio(hitSound, false);
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        Instantiate(effect, transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2), Quaternion.identity);
        cam.orthographicSize = 2;
    }
    void UIUpdate()
    {
        var ui = UIManager.instance;
        statParent.anchoredPosition
        = ui.cam.WorldToScreenPoint(transform.position + (new Vector3(-2f, 0) * (isLeft ? 1 : -1)));
        hpImage.fillAmount = (float)hp / maxHP;
        if (dmgDelayCurTime <= 0)
        {
            hpAnimImage.fillAmount = Mathf.MoveTowards(hpAnimImage.fillAmount, hpImage.fillAmount, Time.deltaTime);
            shieldAnimImage.fillAmount = Mathf.MoveTowards(shieldAnimImage.fillAmount, shieldImage.fillAmount, Time.deltaTime);
        }
        else dmgDelayCurTime -= Time.deltaTime;
        shieldImage.fillAmount = (float)shield / maxShield;
    }
    public void InitCurSkillDamage(RequestSkill skill)
    {
        curDamage = Mathf.FloorToInt((float)Random.Range(skill.minDamage, skill.maxDamage + 1) / skill.attackCount);
    }
    public RequestSkill ConvertRequest(Skill skill)
    {
        RequestSkill newRequest = new RequestSkill();
        newRequest.animation = Resources.Load<AnimationClip>($"Animation/{unitName.Trim()}/{skill.animationName.Trim()}");
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
        var u = UIManager.instance;
        if (shield <= damage)
        {
            Damage(damage - shield);
            if(shield > 0) FatalDamage();
            shield = 0;
        }
        else
        {
            dmgDelayCurTime = dmgDelayTime;
            shield -= damage;
            u.DamageText(damage, transform.position);
            StartCoroutine(HitAnimation(curDamage));
        }
    }
    protected abstract void FatalDamage();
    public void Damage(int damage)
    {
        int totalDmg = 0;
        if (shield <= 0)
        {
            totalDmg = Mathf.FloorToInt(2f * damage);
            hp -= totalDmg;
        }
        else
        {
            totalDmg = damage;
            hp -= totalDmg;
        }
        dmgDelayCurTime = dmgDelayTime;
        if(totalDmg >= 12) FatalDamage();
        UIManager.instance.DamageText(totalDmg, transform.position);
        StartCoroutine(HitAnimation(curDamage));
    }
    IEnumerator HitAnimation(int damage)
    {
        var wait = new WaitForSeconds(0.1f);
        var curPos = transform.position;
        int[] dir = { -1, 1 };

        var addValue = Mathf.InverseLerp(0, 30, Mathf.Clamp(damage, 0, 30)) + 1;
        var value = new Vector3(dir[Random.Range(0, 2)] * 0.5f * addValue, 0, 0);

        transform.position += value;
        yield return wait;
        transform.position -= value * 1.5f;
        yield return wait;
        transform.position = curPos;
    }
}
