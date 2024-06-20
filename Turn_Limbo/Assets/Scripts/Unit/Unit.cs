using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public struct ActionPerformInfo
{
    public int actionValue;
    public int damage;
    public string skillName;
    public bool tryDestorySkill;
    public ActionType attackType;
}

public struct ActionPerformResult
{
    public int decreasedDamage;
    public int takenDamange;
    public int takenHpDamage;
    public int takenShieldDamage;
    public bool isFatalAttack;
    public bool isSkillDestoryed;
}

public abstract class Unit : MonoBehaviour
{
    //singleton
    private AttackManager attackManager => AttackManager.instance;
    private DataManager dataManager => DataManager.instance;

    //inspector
    [SerializeField] protected int maxHP;
    [SerializeField] protected int maxShield;
    [SerializeField] protected Sprite[] actionSprites;

    //field
    protected bool shieldBreak;
    protected bool isEndAnimation;
    protected int hp;
    protected int shield;
    protected Buffs buffs = new();
    protected Animator anim;
    protected SpriteRenderer sr;
    protected List<string> actionQueue = new List<string>();
    protected string curUseAction;

    //property
    protected Action_Base CurAction => attackManager.ActionTable[curUseAction];
    protected ActionInfo CurActionInfo => dataManager.loadData.ActionInfos[curUseAction];
    
    public string unitName => gameObject.name;
    public string CurUseAction => curUseAction;
    public bool HasAction => !string.IsNullOrEmpty(CurUseAction);
    public IReadOnlyList<string> ActionQueue => actionQueue;
    public bool IsEndAnimation => isEndAnimation;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // isLeft = transform.position.x < 0;
    }

    public virtual void OnTurnStart()
    {
        anim.enabled = true;

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

    public void Attack()
    {
        // StartCoroutine(Routine());
        // IEnumerator Routine()
        // {
        //     //get skill from queue
        //     curUseAction = actionQueue[0];
        //     actionQueue.RemoveAt(0);

        //     //0, 2, 4, 8
        //     sr.sprite = actionSprites[((int)CurActionInfo.actionType - 1) * 2];
        // }
    }

    public void Damage(ActionPerformResult result)
    {

    }

    public ActionPerformInfo GetPerformInfo()
    {
        return new();
    }

    public ActionPerformResult Simulate(ActionPerformInfo requestSkill)
    {
        //Debug.Log(damage);
        var response = new ActionPerformResult();
        var damage = Mathf.RoundToInt(requestSkill.damage * buffs.GetBuffValue(Buffs.Key.AttackUp).power);

        if (shield <= 0)
        {
            response.takenDamange = Mathf.FloorToInt(2f * damage);
            hp -= response.takenDamange;
        }
        else
        {
            shield -= damage;
            if(shield != 0) damage = 0;

            response.takenDamange = damage;
            hp -= response.takenDamange;
        }

        return response;
    }
}
