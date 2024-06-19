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
    public Skill_Base effect;
    public Icon insertImage;
    public AnimationClip animation;
    public Unit.ActionType actionType;
    public string effect_desc;
    public string skill_desc;
    public PropertyType propertyType;
}

public class Buff
{
    public Buff_Base buff;
    public PropertyType type;
    public Image insertImage;
    public int stack;
    public int count;

    public Buff(Buff_Base _curBuff, int _stack, int _count, PropertyType _type = PropertyType.AllType)
    {
        buff = _curBuff;
        type = _type;
        stack = _stack;
        count = _count;
    }
}

public enum BuffTiming
{
    turnStart,
    turnEnd,
    battleEnd,
}

public enum PropertyType
{
    AllType,
    Slash,
    Hit,
    Penetrate,
    Defense
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

    public List<Buff> curBuff = new();
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
    public AudioClip[] hitSound;
    public Sprite Uniticon;
    public SkillEffect skillInfo;
    public float maxColorTime;
    public float colorTime;
    public Color color;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public bool isDialogue;
    [HideInInspector] public Sequence iconAnim;

    public UnitUI unitUI;
    [HideInInspector] public float dmgDelayTime;
    [SerializeField] private float dmgDelayCurTime;

    private void Awake()
    {
        maxColorTime = 0.25f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        isLeft = target.transform.position.x > transform.position.x;
        unitUI.InitUnit();
    }
    protected virtual void Update()
    {
        if (colorTime > 0)
        {
            spriteRenderer.color = Color.Lerp(Color.white, color, colorTime / maxColorTime);
            colorTime -= Time.deltaTime;
        }
        unitUI.UIUpdate(transform, hp, maxHP, shield, maxShield, ref dmgDelayCurTime, isLeft);
    }
    public virtual void TurnInit()
    {
        UseBuff(BuffTiming.battleEnd);
        curBuff = ClearBuffList(curBuff, true);

        Debug.Log($"{this} : {curBuff.Count} / {usedBuff.Count}");

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

    public virtual void UseBuff(BuffTiming timing)
    {
        for (int i = 0; i < curBuff.Count; i++)
        {
            if (curBuff[i].buff.timing != timing) return;

            curBuff[i].buff.Use(this, curBuff[i].stack, curBuff[i].type);

            curBuff[i].count--;
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
                        target.color = Color.cyan;
                        target.colorTime = target.maxColorTime;
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
        if (ui.isCamRotate) ui.camRotZ -= Random.Range(UIManager.instance.camRotZ / 2, UIManager.instance.camRotZ * 2.5f);
        SoundManager.instance.SetAudio(hitSound[Random.Range(0, hitSound.Length)], false, Random.Range(0.75f, 1.25f));
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        Instantiate(effect, transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2), Quaternion.identity);
        cam.orthographicSize = 2;
    }

    public List<Buff> ClearBuffList(List<Buff> list, bool allClaer = false)
    {
        List<Buff> temp = new();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].count != 0 && !allClaer)
            {
                if (list[i].insertImage == null)
                {
                    list[i].insertImage = UIManager.instance.AddImage(list[i].buff.buffIcon, unitUI.requestBuffParent);
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
        newRequest.effect_desc = skill.effect_desc;
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
        colorTime = maxColorTime;
        color = Color.yellow;
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
            DamagePush(dir, damage);
            //StartCoroutine(HitAnimation(curDamage));
            u.DamageText(totalDmg, transform.position, this);
        }
    }
    protected abstract void DamageLogs(int damage);
    protected abstract void FatalDamage();
    public void Damage(int damage, Vector3 dir)
    {
        colorTime = maxColorTime;
        color = Color.red;
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
        UIManager.instance.DamageText(totalDmg, transform.position, this);
    }
    void DamagePush(Vector3 dir, int damage)
    {
        var addPos = dir * damage * 0.3f;
        //print($"{gameObject.name} {addPos}");
        transform.DOMove(transform.position + addPos, 0.2f);
    }
}
