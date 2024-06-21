using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public struct ActionPerformInfo
{
    public float gameTime;
    public int actionValue;
    public float damage;
    public float damageMultiply;
    public string actionName;
    public bool tryDestorySkill;
    public ActionType actionType;
}

public struct ActionPerformResult
{
    public float gameTime;
    public int decreasedDamage;
    public int takenDamange;
    public int takenHpDamage;
    public int takenShieldDamage;
    public bool isFatalAttack;
    public bool isSkillDestoryed;
}

public class Unit : MonoBehaviour
{
    //singleton
    private GameManager attackManager => GameManager.instance;
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
    protected float onWayAttackSpeedStack = 1;
    protected Buffs buffs;
    protected Animator anim;
    protected SpriteRenderer sr;
    protected List<string> actionQueue = new List<string>();
    protected string curUseAction;
    protected ActionPerformInfo actionPerformInfo;
    protected ActionPerformResult actionPerformResult;

    //property
    protected Action_Base CurAction => attackManager.ActionTable[CurActionInfo.script];
    protected ActionInfo CurActionInfo => dataManager.loadData.ActionInfos[curUseAction];
    
    public int MaxHp => maxHP;
    public int MaxShield => maxShield;
    public int Hp => hp;
    public int Shield => shield;
    public string unitName => gameObject.name;
    public string CurUseAction => curUseAction;
    public bool HasAction => !string.IsNullOrEmpty(CurUseAction);
    public IReadOnlyList<string> ActionQueue => actionQueue;
    public bool IsEndAnimation => isEndAnimation;

    protected virtual void Awake()
    {
        buffs = new(this);
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        hp = maxHP;
        shield = maxShield;
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

    public void PrepareAction()
    {
        if(actionQueue.Count == 0) return;
        
        curUseAction = actionQueue[0];
        actionQueue.RemoveAt(0);
    }

    public void PerformAction(Unit target, bool isOneWay = false)
    {
        Debug.Log("Perform");
        isEndAnimation = false;
        StartCoroutine(Routine(target, isOneWay));
        
        IEnumerator Routine(Unit target, bool isOneWay)
        {
            //initialize
            CurAction.Use(this, dataManager.actionLevels[curUseAction], target);
            CurAction.OnAttackStart();

            //info
            var info = new ActionPerformInfo();
            info.gameTime = Time.time;
            info.actionValue = CurAction.GetActionValue();
            
            CurAction.OnIncreaseDamage(ref info);

            info.damage = info.actionValue * info.damageMultiply;

            actionPerformInfo = info;

            //sprite
            anim.enabled = false;
            sr.sprite = actionSprites[((int)CurActionInfo.actionType - 1) * 2];
            yield return new WaitForSeconds(0.3f / onWayAttackSpeedStack);
            sr.sprite = actionSprites[((int)CurActionInfo.actionType - 1) * 2 + 1];

            if(isOneWay) onWayAttackSpeedStack += 0.5f;
            else onWayAttackSpeedStack = 1;

            if(onWayAttackSpeedStack > 2.5f)
                onWayAttackSpeedStack = 2.5f;

            //damage
            actionPerformResult = target.React(info);
            CurAction.OnPerformAction();

            yield return new WaitForSeconds(0.15f / onWayAttackSpeedStack);
            isEndAnimation = true;
        }
    }

    public ActionPerformResult React(ActionPerformInfo info)
    {
        Debug.Log("React");
        var response = new ActionPerformResult();
        return response;
    }
}
