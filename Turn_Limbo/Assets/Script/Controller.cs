using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int minDamage;
    public int maxDamage;
    public int attackCount;
    public int keyIndex;
    public SkillScript effect;
    public Sprite icon;
    public AnimationClip animation;
    public Unit.ActionType actionType;
    public Unit.PropertyType propertyType;
}

public class Controller : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    public Vector3 movePos;
    public int cursorIndex;
    public Image cursorImage;
    public List<Skill> inputLists = new();

    public float curTime;
    public int useAbleCoin;

    public bool isAttack;

    public VolumeProfile volume;
    [HideInInspector] public ChromaticAberration glitch;
    [HideInInspector] public DepthOfField depth;
    public Sprite test;

    [SerializeField] protected float AnimTime;

    public AudioClip hitSound;
    public AudioClip[] addSkillSound;

    public Dictionary<int, List<Skill>> inputs = new();
    private readonly KeyCode[] cursorKeys = { KeyCode.LeftArrow, KeyCode.RightArrow };
    private void Awake()
    {
        volume.TryGet(out glitch);
        volume.TryGet(out depth);
    }
    void Start()
    {
        TurnReset();
        player.hitSound = hitSound;
        enemy.hitSound = hitSound;
        player.AnimTime = AnimTime;
        enemy.AnimTime = AnimTime;

        depth.focalLength.value = 1;

        cursorIndex = 0;
    }
    public void TurnReset()
    {
        curTime = 10;
        useAbleCoin += player.addCoin;
        UIManager.instance.ChangeCoinSkillImg();

        player.TurnInit();
        enemy.TurnInit();

        foreach (var n in player.usedBuff) IconAnim(player, n.insertImage);
        player.usedBuff.Clear();
        foreach (var n in enemy.usedBuff) IconAnim(enemy, n.insertImage);
        enemy.usedBuff.Clear();
    }
    public void TurnEnd()
    {
        InitEnemy();
        TurnReset();
    }
    public void InitEnemy()
    {
        var d = DataManager.instance;
        var skillArray = inputs.Values.ToArray();
        var coinCount = Random.Range(enemy.requestMinCount, enemy.requestMaxCount + 1);
        for (int i = 0; i < coinCount; i++)
        {
            AddRequest(enemy, d.SkillList[d.skillEffects[1].holdIndex[enemy.skillCurCount % d.skillEffects[1].holdIndex.Count]]);
            enemy.skillCurCount++;
        }
    }
    void Update()
    {
        int blurValue = 0;
        CheckInput();
        if (Input.GetKeyDown(KeyCode.A) && !isAttack)
        {
            UseAttack();
        }
        if (isAttack) blurValue = 500;
        else
        {
            blurValue = 1;
            curTime -= Time.deltaTime;
            if (curTime <= 0) UseAttack();
        }
        UIUpdate(player);
        UIUpdate(enemy);
        depth.focalLength.value = Mathf.MoveTowards(depth.focalLength.value, blurValue, Time.deltaTime * 500);
        glitch.intensity.value = Mathf.MoveTowards(glitch.intensity.value, 0, Time.deltaTime);
    }
    void UIUpdate(Unit character)
    {
        var ui = UIManager.instance;
        character.requestUIParent.anchoredPosition
        = !isAttack ? ui.cam.WorldToScreenPoint(character.transform.position
        + new Vector3((3 - (5 - ui.cam.orthographicSize)) * (character.isLeft ? 1 : -1), 3))
        : new Vector3(ui.cam.WorldToScreenPoint(character.transform.position).x, 900);

        character.requestBuffParent.anchoredPosition
        = !isAttack ? ui.cam.WorldToScreenPoint(character.transform.position
        + new Vector3((3 - (5 - ui.cam.orthographicSize)) * (character.isLeft ? 1 : -1), 4))
        : new Vector3(ui.cam.WorldToScreenPoint(character.transform.position).x, 1000);
    }

    public void InitBtn()
    {
        for (int i = 0; i < 3; i++)
        {
            UIManager.instance.NextImage(i, inputs[i][0].icon, inputs[i][1].icon);
        }
    }
    public void AddRequest(Unit target, Skill addSkill)
    {
        if (!isAttack)
        {
            var newSkill = target.ConvertRequest(addSkill);
            newSkill.insertImage = UIManager.instance.AddImage(newSkill.icon, target.requestUIParent);
            target.attackRequest.Enqueue(newSkill);
        }
    }
    private void CheckInput()
    {
        int[] plusIndex = { -1, 1 };
        if (useAbleCoin <= 0) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            var input = inputs[cursorIndex];
            AddRequest(player, input[0]);
            SwapSkills(input);
            useAbleCoin--;
            UIManager.instance.ChangeCoinSkillImg();
            UIManager.instance.NextImage(cursorIndex, input[0].icon, input[1].icon);
            SoundManager.instance.SetAudio(addSkillSound[Random.Range(0, addSkillSound.Length)], false);
        }
        for (int i = 0; i < cursorKeys.Length; i++)
        {
            if (Input.GetKeyDown(cursorKeys[i]))
            {
                var ui = UIManager.instance;
                cursorIndex += plusIndex[i];
                if (cursorIndex <= -1) cursorIndex = ui.keys.Length - 1;
                else if (cursorIndex >= ui.keys.Length) cursorIndex = 0;
                cursorImage.transform.position = ui.keys[cursorIndex].transform.position;
            }
        }
    }

    public void UseAttack()
    {
        UIManager.instance.ActiveBtn(false);
        StartCoroutine(Attack());
    }

    IEnumerator FirstAttackMove(Unit unit)
    {
        movePos = Vector3.Lerp(unit.transform.position, unit.target.transform.position, 0.5f);

        yield return unit.transform.DOMoveX(movePos.x - (2 * (unit.isLeft ? 1 : -1)), 0.5f)
        .SetEase(Ease.OutCubic).WaitForCompletion();
    }

    IEnumerator Attack()
    {
        isAttack = true;
        Time.timeScale = 1.5f;
        UIManager.instance.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        StartCoroutine(FirstAttackMove(player));
        yield return StartCoroutine(FirstAttackMove(enemy));
        var attackCount = Mathf.Max(player.attackRequest.Count, enemy.attackRequest.Count);
        for (int i = 0; i < attackCount; i++)
        {
            // yield return new WaitForSeconds(skill.animation.length + 0.1f);
            float waitTime = Mathf.Max(AttackInit(player), AttackInit(enemy));
            player.BuffSetting(); enemy.BuffSetting();
            AttackStart(player); AttackStart(enemy);
            player.curSkill.effect?.End(player, player.target);
            enemy.curSkill.effect?.End(enemy, enemy.target);
            BuffClaer(player); BuffClaer(enemy);
            yield return new WaitForSeconds(waitTime);
        }

        yield return new WaitForSeconds(0.5f);
        UIManager.instance.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);

        player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        yield return enemy.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        Time.timeScale = 1f;
        isAttack = false;
        UIManager.instance.ActiveBtn(true);
        TurnEnd();
    }

    float AttackInit(Unit unit)
    {
        if (unit.attackRequest.Count <= 0) { unit.SkillInit(unit.nullSkill); return 0; }
        var skill = unit.SkillChange();
        unit.SkillInit(skill);

        return skill.animation.length;
    }

    void AttackStart(Unit unit)
    {
        var skill = unit.curSkill;
        if (unit.curSkill.actionType == Unit.ActionType.none) { return; }
        unit.InitCurSkillDamage(skill.minDamage, skill.maxDamage, skill.attackCount);
        unit.curSkill.effect?.Setting(unit, unit.target);
        unit.anim.Play(skill.animation.name);
        IconAnim(unit, skill.insertImage);
    }

    void BuffClaer(Unit unit)
    {
        unit.ClaerBuff();

        foreach (var n in unit.usedBuff)
        {
            Debug.Log($"{unit.name} {n.curBuff} {n.insertImage == null}");
            IconAnim(unit, n.insertImage);
        }

        unit.usedBuff.Clear();
    }

    void IconAnim(Unit unit, Image insertImage)
    {
        unit.iconAnim = DOTween.Sequence();

        unit.iconAnim.Append(insertImage.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutQuint));
        unit.iconAnim.Append(insertImage.transform.DOScale(0, 0.3f).SetEase(Ease.OutQuint));
        unit.iconAnim.AppendCallback(() =>
        {
            Destroy(insertImage.gameObject);
        });
        unit.iconAnim.Play();
    }

    public void SwapSkills(List<Skill> key)
    {
        var useSkills = key[0];
        key.RemoveAt(0);
        key.Add(useSkills);
    }
}
