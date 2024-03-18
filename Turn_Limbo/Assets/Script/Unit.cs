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
    public Unit.PropertyType propertyType;
}

public class Buff
{
    public Buff_Base curBuff;
    public Unit.PropertyType type;
    public int stack;
    public int count;

    public Buff(Buff_Base _curBuff, int _stack, int _count, Unit.PropertyType _type = Unit.PropertyType.AllType)
    {
        curBuff = _curBuff;
        type = _type;
        stack = _stack;
        count = _count;
    }
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
        AllType,
        Slash,
        Hit,
        Penetrate,
        Defense
    }
    [HideInInspector] public List<Buff> turnStart = new();
    [HideInInspector] public List<Buff> inUse = new();
    [HideInInspector] public List<Buff> turnEnd = new();
    [HideInInspector] public List<Buff> battleEnd = new();

    public Queue<RequestSkill> attackRequest = new Queue<RequestSkill>();
    public int hp;
    public int maxHP;

    public int shield;
    public int maxShield;

    public bool shieldBreak;

    public int curDamage;
    public float attack_Drainage;
    public float defense_Drainage;
    public bool isAttack;

    public Unit target;
    public RequestSkill nextSkill;
    public RequestSkill curSkill;
    public RequestSkill usedSkill;
    public readonly RequestSkill nullSkill = new RequestSkill()
    {
        actionType = ActionType.none
    };
    public ParticleSystem effect;
    public AudioClip hitSound;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public Sequence iconAnim;
    [HideInInspector] public int coin = 3;

    public RectTransform requestUIParent;
    [SerializeField] protected RectTransform statParent;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image hpAnimImage;
    [SerializeField] protected Image shieldImage;
    [SerializeField] protected Image shieldAnimImage;
    [HideInInspector] public float AnimTime;
    [SerializeField] private float AnimCurTime;

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
        for (int i = 0; i < battleEnd.Count; i++)
        {
            var curBuff = battleEnd[i];

            curBuff.curBuff.Use(this, curBuff.stack, curBuff.type);

            curBuff.count--;
            if (curBuff.count <= 0) battleEnd[i] = null;
        }

        turnStart.Clear();
        turnEnd.Clear();
        inUse.Clear();
        battleEnd.Clear();

        coin = 3;
        isAttack = true;
        nextSkill = new();

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
    public void SkillInit(RequestSkill skill)
    {
        attack_Drainage = 1;
        defense_Drainage = 1;

        usedSkill = curSkill;
        curSkill = skill;

        Debug.Log($"{this.name} >>> {usedSkill.skillName}_ _{curSkill.skillName}");
    }

    public virtual void AttackStart(RequestSkill skill)
    {
        for (int i = 0; i < turnStart.Count; i++)
        {
            var curBuff = turnStart[i];

            curBuff.curBuff.Use(this, curBuff.stack, curBuff.type);

            curBuff.count--;
        }

        curSkill.effect.Setting(this, target);
    }
    public virtual void Attacking()
    {
        Debug.Log($">{this.name} {curSkill.skillName} {usedSkill.skillName}");
        if (isAttack)
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
            curSkill.effect.Attack(this, target);
        }
        else Debug.Log($"{this.name} Attack Break!!");
        var cam = UIManager.instance.cam;
        cam.transform.position = cam.transform.position + ((Vector3)Random.insideUnitCircle.normalized * 1);
        SoundManager.instance.SetAudio(hitSound, false);
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        Instantiate(effect, transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2), Quaternion.identity);
        cam.orthographicSize = 2;
    }
    public virtual void AttackEnd(RequestSkill skill)
    {
        curSkill.effect?.End(this, target);

        turnStart = ClearList(turnStart);
        turnEnd = ClearList(turnEnd);
        inUse = ClearList(inUse);
        battleEnd = ClearList(battleEnd);

        isAttack = true;
    }
    List<Buff> ClearList(List<Buff> list)
    {
        List<Buff> temp = new();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].count != 0) temp.Add(list[i]);
        }

        return temp;
    }
    void UIUpdate()
    {
        var ui = UIManager.instance;
        statParent.anchoredPosition
        = ui.cam.WorldToScreenPoint(transform.position + (new Vector3(-2f, 0) * (isLeft ? 1 : -1)));
        hpImage.fillAmount = (float)hp / maxHP;
        if (AnimCurTime <= 0)
        {
            hpAnimImage.fillAmount = Mathf.MoveTowards(hpAnimImage.fillAmount, hpImage.fillAmount, Time.deltaTime);
            shieldAnimImage.fillAmount = Mathf.MoveTowards(shieldAnimImage.fillAmount, shieldImage.fillAmount, Time.deltaTime);
        }
        else AnimCurTime -= Time.deltaTime;
        shieldImage.fillAmount = (float)shield / maxShield;

        requestUIParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
    }
    public void InitCurSkillDamage(int min, int max, int count)
    {
        curDamage = Mathf.FloorToInt((float)UnityEngine.Random.Range(min, max + 1) * attack_Drainage / count);
        //Debug.Log($"{this.name} Damage : {curDamage} {attack_Drainage}");
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
        newRequest.propertyType = skill.propertyType;
        return newRequest;
    }

    public RequestSkill SkillChange()
    {
        RequestSkill temp = new();

        if (attackRequest.Count != 0)
        {
            temp = attackRequest.Dequeue();
        }

        if (nextSkill.skillName != null)
        {
            temp.skillName = nextSkill.skillName;
            temp.minDamage = nextSkill.minDamage;
            temp.maxDamage = nextSkill.maxDamage;
            temp.attackCount = nextSkill.attackCount;
            temp.icon = nextSkill.icon;
            temp.effect = nextSkill.effect;
            temp.animation = nextSkill.animation;
            temp.actionType = nextSkill.actionType;
            temp.propertyType = nextSkill.propertyType;

            temp.insertImage.sprite = temp.icon;

            nextSkill = new();

            Debug.Log($"IN {temp.skillName}");
        }

        return temp;
    }

    public void ShieldDamage(int damage)
    {
        var u = UIManager.instance;
        //Debug.Log($"{defense_Drainage} {damage} {damage * defense_Drainage} {Mathf.RoundToInt(damage * defense_Drainage)}");
        damage = Mathf.RoundToInt(damage * defense_Drainage);
        if (shield <= damage)
        {
            Damage(damage - shield);
            if (shield > 0) FatalDamage();
            shield = 0;
        }
        else
        {
            AnimCurTime = AnimTime;
            shield -= damage;
            u.DamageText(damage, transform.position);
            StartCoroutine(HitAnimation(curDamage));
        }
    }
    protected abstract void FatalDamage();
    public void Damage(int damage)
    {
        //Debug.Log(damage);
        int totalDmg = 0;
        damage = Mathf.RoundToInt(damage * defense_Drainage);
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
        //Debug.Log($"{defense_Drainage} {damage} {totalDmg}");
        AnimCurTime = AnimTime;
        if (totalDmg >= 12) FatalDamage();
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
        //print($"{addValue},{value.magnitude}");

        transform.position += value;
        yield return wait;
        transform.position -= value * 1.5f;
        yield return wait;
        transform.position = curPos;
    }
}
