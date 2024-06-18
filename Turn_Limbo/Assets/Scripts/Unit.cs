using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public struct AttackInfomation
{
    public int index;
    public int damage;
    public string skillName;
    public PropertyType propertyType;
}

public struct AttackResult : IEqualityComparer<AttackResult>
{
    public float gameTime;
    public int takenDamange;
    public bool isShieldAttacked;
    public bool isFatalAttack;

    public bool Equals(AttackResult x, AttackResult y)
    {
        return x.gameTime.Equals(y.gameTime);
    }

    public int GetHashCode(AttackResult obj)
    {
        return obj.gameTime.GetHashCode();
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

    public Buffs buffs = new();

    public List<AttackInfomation> attackRequest = new List<AttackInfomation>();
    public string unitName => gameObject.name;
    public int hp;
    public int maxHP;
    public int hpLimit;
    public int shield;
    public int maxShield;
    public bool shieldBreak;

    public ParticleSystem effect;
    public AudioClip[] hitSound;
    public Sprite Uniticon;
    public SkillEffect skillInfo;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public Sequence iconAnim;

    [HideInInspector] public float dmgDelayTime;
    public float dmgDelayCurTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        isLeft = transform.position.x < 0;
    }

    public virtual void OnTurnStart()
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


    public virtual void Attacking()
    {
        // var cam = ui.cam;
        // cam.transform.position = cam.transform.position + ((Vector3)Random.insideUnitCircle.normalized * 1);
        // if (ui.isCamRotate) ui.camRotZ -= Random.Range(UIManager.instance.camRotZ / 2, UIManager.instance.camRotZ * 2.5f);
        // SoundManager.instance.SetAudio(hitSound[Random.Range(0,hitSound.Length)], false, Random.Range(0.75f, 1.25f));
        //Instantiate(curSkill.effect.Hitparticles[0],transform.position,Quaternion.identity);
        // Instantiate(effect, transform.position + (Vector3.right * (isLeft ? 1 : -1) * 2), Quaternion.identity);
        // cam.orthographicSize = 2;
    }

    public AttackResult Damage(AttackInfomation requestSkill, Vector3 dir)
    {
        //Debug.Log(damage);
        var response = new AttackResult() { gameTime = Time.time };
        var damage = Mathf.RoundToInt(requestSkill.damage * buffs.GetBuffValue(Buffs.Key.AttackUp).ratio);

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

        // if (hpLimit != 0 && hp <= hpLimit)
        // {
        //     isDialogue = true;
        //     hp = hpLimit;
        // }
        //Debug.Log($"{defense_Drainage} {damage} {totalDmg}");
        DamagePush(dir, damage);
        dmgDelayCurTime = dmgDelayTime;
        // UIManager.instance.DamageText(totalDmg, transform.position, this);
        return response;
    }

    void DamagePush(Vector3 dir, int damage)
    {
        var addPos = dir * damage * 0.3f;
        //print($"{gameObject.name} {addPos}");
        transform.DOMove(transform.position + addPos, 0.2f);
    }
}
