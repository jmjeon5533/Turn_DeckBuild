using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public struct RequestSkill
{
    public int index;
    public string skillName;
    public int[] cost;
    public int[] minDamage;
    public int[] maxDamage;
    public int level;
    public int attackCount;
    public Sprite icon;
    public SkillScript effect;
    public Icon insertImage;
    public AnimationClip animation;
    public Unit.ActionType actionType;
    public string explain;
    public Unit.PropertyType propertyType;
}

public class Buff
{
    public Buff_Base curBuff;
    public Unit.PropertyType type;
    public Image insertImage;
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
    public List<Buff> turnStart = new();
    public List<Buff> turnEnd = new();
    public List<Buff> battleEnd = new();
    public List<Buff> usedBuff = new();

    public List<RequestSkill> attackRequest = new List<RequestSkill>();
    public string unitName => gameObject.name;
    public int hp;
    public int maxHP;
    public int hpLimit;

    public int shield;
    public int maxShield;

    public bool shieldBreak;

    public int curDamage;
    public int curAttackCount;
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
    public Sprite Uniticon;
    public SkillEffect skillInfo;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public bool isDialogue;
    [HideInInspector] public Sequence iconAnim;

    public RectTransform requestUIParent;
    public RectTransform requestBuffParent;
    public RectTransform statParent;
    [SerializeField] protected GameObject status;
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
        InitUnit();
    }
    public virtual void InitUnit()
    {
        hpAnimImage = statParent.GetChild(1).GetComponent<Image>();
        hpImage = hpAnimImage.transform.GetChild(0).GetComponent<Image>();

        shieldAnimImage = statParent.GetChild(3).GetComponent<Image>();
        shieldImage = shieldAnimImage.transform.GetChild(0).GetComponent<Image>();
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
        }
        turnStart = ClearBuffList(turnStart, true);
        turnEnd = ClearBuffList(turnEnd, true);
        battleEnd = ClearBuffList(battleEnd, true);

        isAttack = true;
        nextSkill = nullSkill;
        usedSkill = nullSkill;

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
        isAttack = true;
        attack_Drainage = 1;
        defense_Drainage = 1;

        usedSkill = curSkill;
        curSkill = skill;
        //Debug.Log($"{this.name} >>> {usedSkill.skillName}_ _{curSkill.skillName} / usedBuffList : {usedBuff.Count}");
    }

    public virtual void BuffSetting()
    {
        for (int i = 0; i < turnStart.Count; i++)
        {
            var curBuff = turnStart[i];

            curBuff.curBuff.Use(this, curBuff.stack, curBuff.type);

            curBuff.count--;
        }
    }

    public virtual void Attacking()
    {
        var ui = UIManager.instance;
        //Debug.Log($">{this.name} {curSkill.skillName} {usedSkill.skillName}");
        if (isAttack)
        {
            Vector3 dmgDir = (target.transform.position - transform.position).normalized;
            switch (target.curSkill.actionType)
            {

                case ActionType.none:
                    {
                        target.Damage(curDamage, dmgDir);
                    }
                    break;
                case ActionType.Attack:
                    {
                        target.ShieldDamage(curDamage, dmgDir);
                    }
                    break;
                case ActionType.Defence:
                    {
                        target.Damage(Mathf.Clamp(curDamage - Mathf.FloorToInt(target.curDamage / curAttackCount), 0, 999), dmgDir);
                    }
                    break;
                case ActionType.Dodge:
                    {
                        if (target.curDamage < curDamage)
                        {
                            target.Damage(curDamage, dmgDir);
                        }
                        break;
                    }
            }
        }
        else Debug.Log($"{this.name} Attack Break!!");
        var cam = ui.cam;
        cam.transform.position = cam.transform.position + ((Vector3)Random.insideUnitCircle.normalized * 1);
        if(ui.isCamRotate) ui.camRotZ -= Random.Range(UIManager.instance.camRotZ / 2, UIManager.instance.camRotZ * 2.5f);
        SoundManager.instance.SetAudio(hitSound, false);
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        Instantiate(effect, transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2), Quaternion.identity);
        cam.orthographicSize = 2;
    }

    public virtual void ClaerBuff()
    {
        turnStart = ClearBuffList(turnStart);
        turnEnd = ClearBuffList(turnEnd);
        battleEnd = ClearBuffList(battleEnd);
    }

    List<Buff> ClearBuffList(List<Buff> list, bool allClaer = false)
    {
        List<Buff> temp = new();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].count != 0 && !allClaer)
            {
                if (list[i].insertImage == null)
                {
                    list[i].insertImage = UIManager.instance.AddImage(list[i].curBuff.buffIcon, requestBuffParent);
                    //Debug.Log($"AddImage {this.name} {list[i].curBuff} {list[i].insertImage == null}");
                }
                //Debug.Log($"AddList {this.name} {list[i].curBuff} {list[i].insertImage == null}");
                temp.Add(list[i]);
            }
            else
            {
                //Debug.Log($"Die : {this.name} {list[i].curBuff} {list[i].insertImage == null} {allClaer} {list[i].count}");
                usedBuff.Add(list[i]);
            }
        }
        return temp;
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

        requestUIParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
        requestBuffParent.localScale = Vector3.one * (1 + (5 - ui.cam.orthographicSize) * 0.3f);
    }
    public void InitCurSkillDamage(int min, int max, int count)
    {
        //Debug.Log($"{this.name} Damage : {curDamage} {attack_Drainage}");
        var damage = (float)Random.Range(min, max + 1);
        curDamage = Mathf.FloorToInt(damage * attack_Drainage / count);
        curAttackCount = count;
    }
    public RequestSkill ConvertRequest(Skill skill)
    {
        RequestSkill newRequest = new RequestSkill();
        newRequest.animation = Resources.Load<AnimationClip>($"Animation/{unitName.Trim()}/{skill.animationName.Trim()}");
        newRequest.cost = skill.cost;
        newRequest.minDamage = skill.minDamage;
        newRequest.maxDamage = skill.maxDamage;
        newRequest.level = skill.level;
        newRequest.actionType = skill.actionType;
        newRequest.attackCount = skill.attackCount;
        newRequest.effect = skill.effect;
        newRequest.explain = skill.explain;
        newRequest.icon = skill.icon;
        newRequest.skillName = skill.skillName;
        newRequest.propertyType = skill.propertyType;
        return newRequest;
    }

    public RequestSkill SkillChange()
    {
        RequestSkill temp = attackRequest[0];
        attackRequest.RemoveAt(0);

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

            temp.insertImage.iconImage.sprite = temp.icon;

            nextSkill = new();

            Debug.Log($"IN {temp.skillName}");
        }

        return temp;
    }

    public void ShieldDamage(int damage, Vector3 dir)
    {
        var u = UIManager.instance;
        //Debug.Log($"{defense_Drainage} {damage} {damage * defense_Drainage} {Mathf.RoundToInt(damage * defense_Drainage)}");
        damage = Mathf.RoundToInt(damage * defense_Drainage);
        if (shield <= damage)
        {
            Damage(damage - shield, dir);
            DamageLogs(damage - shield);
            if (shield > 0) FatalDamage();
            shield = 0;
        }
        else
        {
            dmgDelayCurTime = dmgDelayTime;
            var totalDmg = damage;
            shield -= totalDmg;
            DamageLogs(totalDmg);
            u.DamageText(totalDmg, transform.position);
            DamagePush(dir, damage);
            //StartCoroutine(HitAnimation(curDamage));
        }
    }
    protected abstract void DamageLogs(int damage);
    protected abstract void FatalDamage();
    public void Damage(int damage, Vector3 dir)
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

        if (hpLimit != 0 && hp <= hpLimit)
        {
            isDialogue = true;
            hp = hpLimit;
        }
        //Debug.Log($"{defense_Drainage} {damage} {totalDmg}");
        DamagePush(dir, damage);
        dmgDelayCurTime = dmgDelayTime;
        if (totalDmg >= 12) FatalDamage();
        DamageLogs(totalDmg);
        UIManager.instance.DamageText(totalDmg, transform.position);
        //StartCoroutine(HitAnimation(curDamage));
    }
    void DamagePush(Vector3 dir, int damage)
    {
        var addPos = dir * damage * 0.3f;
        print($"{gameObject.name} {addPos}");
        transform.DOMove(transform.position + addPos, 0.2f);
    }
    public void HideUI(bool isOn)
    {
        requestUIParent.gameObject.SetActive(isOn);
        requestBuffParent.gameObject.SetActive(isOn);
        status.SetActive(isOn);
    }
}
