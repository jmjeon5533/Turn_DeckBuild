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
    public string animationName;
    public Unit.ActionType actionType;
    public string explain;
    public Unit.PropertyType propertyType;
}

public class Controller : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    public Vector3 movePos;
    public SpriteRenderer bg;
    public Image keyHoldImage;
    public List<SkillScript> skills = new();
    public List<Skill> inputLists = new();

    public float gameCurTimeCount;
    public float keyHoldTime;
    public int useAbleCoin;

    public bool isGame;
    public bool isTab;
    public bool isAttack;
    public bool isSkillExplain;

    private int lastSign;

    public VolumeProfile volume;
    [HideInInspector] public ChromaticAberration glitch;
    [HideInInspector] public DepthOfField depth;
    [HideInInspector] public ColorAdjustments color;

    [SerializeField] protected float AnimTime;

    public AudioClip hitSound;
    public AudioClip[] addSkillSound;

    public Dictionary<int, List<Skill>> inputs = new();
    private readonly KeyCode[] KEY_CODE = { KeyCode.Q, KeyCode.W, KeyCode.E };
    private void Awake()
    {
        volume.TryGet(out glitch);
        volume.TryGet(out depth);
        volume.TryGet(out color);
    }
    void Start()
    {
        TurnReset();
        UIManager.instance.SetExplain(false);
        player.hitSound = hitSound;
        enemy.hitSound = hitSound;
        player.dmgDelayTime = AnimTime;
        enemy.dmgDelayTime = AnimTime;

        depth.focalLength.value = 1;
        color.postExposure.value = 0;
        color.saturation.value = 0;
    }
    public void TurnReset()
    {
        gameCurTimeCount = 10;

        player.TurnInit();
        enemy.TurnInit();
        useAbleCoin += player.addCoin;
        UIManager.instance.ChangeCoinSkillImg();

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
        var coinCount = Random.Range(enemy.requestMinCount, enemy.requestMaxCount + 1);
        for (int i = 0; i < coinCount; i++)
        {
            AddRequest(enemy, d.SkillList[d.skillEffects[1].holdIndex[enemy.skillCurCount % d.skillEffects[1].holdIndex.Count]]);
            enemy.skillCurCount++;
        }
    }
    void Update()
    {
        UIUpdate(player);
        UIUpdate(enemy);
        int blurValue = 0;
        if (isAttack && isGame) blurValue = 500;
        else
        {
            blurValue = 1;
            gameCurTimeCount -= Time.deltaTime;
            if (gameCurTimeCount <= 0) UseAttack();
        }
        UIUpdate(player);
        UIUpdate(enemy);
        depth.focalLength.value = Mathf.MoveTowards(depth.focalLength.value, blurValue, Time.deltaTime * 375);
        glitch.intensity.value = Mathf.MoveTowards(glitch.intensity.value, 0, Time.deltaTime * 0.75f);
        color.postExposure.value = Mathf.MoveTowards(color.postExposure.value, 0, Time.deltaTime * 0.75f);
        color.saturation.value = Mathf.MoveTowards(color.saturation.value, 0, Time.deltaTime * 60);
        if (!isGame) return;

        CheckInput();
        if (Input.GetKeyDown(KeyCode.A) && !isAttack)
        {
            UseAttack();
        }
        bool isSpace = Input.GetKey(KeyCode.Space) && isAttack;
        isTab = Input.GetKey(KeyCode.Tab) && !isAttack;
        float timeScale;
        if(isAttack)
        {
            timeScale = isSpace ? 0.4f : 1;
        }
        else
        {
            timeScale = isTab ? 0.2f : 1;
        }
        
        Time.timeScale = timeScale;
        Color bgColor = isSpace || isTab ? new Color(0.6f, 0.6f, 0.6f, 1) : Color.white;
        bg.color = bg.color.MoveToward(bgColor, Time.deltaTime * 5f);
    }
    void UIUpdate(Unit character)
    {
        var ui = UIManager.instance;
        var requestPos = !isAttack ? ui.cam.WorldToScreenPoint(character.transform.position
        + new Vector3((2 - (5 - ui.cam.orthographicSize)) * (character.isLeft ? 1 : -1), 2))
        : new Vector3(ui.cam.WorldToScreenPoint(character.transform.position).x, 900);

        character.requestBuffParent.anchoredPosition
        = !isAttack ? ui.cam.WorldToScreenPoint(character.transform.position
        + new Vector3((3 - (5 - ui.cam.orthographicSize)) * (character.isLeft ? 1 : -1), 4))
        : new Vector3(ui.cam.WorldToScreenPoint(character.transform.position).x, 1000);
        character.requestUIParent.anchoredPosition
        = Vector3.Lerp(character.requestUIParent.anchoredPosition, requestPos, 0.05f);

        float scale = 0;
        if (isAttack) scale = 1.5f;
        else scale = 1 + (5 - ui.cam.orthographicSize) * 0.2f;
        character.requestUIParent.localScale = Vector3.one * scale;
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
            target.attackRequest.Add(newSkill);
        }
    }
    private void CheckInput()
    {
        if (useAbleCoin <= 0) return;
        var ui = UIManager.instance;
        if(isTab) return;
        for (int i = 0; i < KEY_CODE.Length; i++)
        {
            if (Input.GetKeyUp(KEY_CODE[i]))
            {
                if (keyHoldTime <= 0.3f && !isSkillExplain)
                {
                    var input = inputs[i];
                    AddRequest(player, input[0]);
                    SwapSkills(input);
                    useAbleCoin--;
                    ui.ChangeCoinSkillImg();
                    ui.NextImage(i, input[0].icon, input[1].icon);
                    SoundManager.instance.SetAudio(addSkillSound[Random.Range(0, addSkillSound.Length)], false);
                }
                keyHoldTime = 0;
                keyHoldImage.fillAmount = Mathf.Clamp(keyHoldTime - 0.5f, 0, 10) / 1f;
            }
            if (Input.GetKeyDown(KEY_CODE[i]) && isSkillExplain)
            {
                if (isSkillExplain)
                {
                    ui.SetExplain(false);
                    isSkillExplain = false;
                }
            }
            if (Input.GetKey(KEY_CODE[i]) && !isSkillExplain)
            {
                keyHoldTime += Time.unscaledDeltaTime;
                keyHoldImage.rectTransform.anchoredPosition = ui.keys[i].rectTransform.anchoredPosition;
                if (keyHoldTime > 1f)
                {
                    isSkillExplain = true;
                    keyHoldTime = 0;
                    ui.SetExplain(true, inputs[i][0], ui.keys[i].rectTransform.anchoredPosition);
                }
                keyHoldImage.fillAmount = Mathf.Clamp(keyHoldTime - 0.3f, 0, 10) / 0.5f;
            }
        }
    }
    public void GameEnd()
    {
        if (player.hp <= 0) GameOver();
        else GameClear();
        Time.timeScale = 0;
    }
    public void GameOver()
    {
        UIManager.instance.SetGameEndUI(false);
    }
    public void GameClear()
    {
        UIManager.instance.SetGameEndUI(true);
        print("게임 ?��리어");
    }

    public void UseAttack()
    {
        UIManager.instance.ActiveBtn(false);
        UIManager.instance.SetExplain(false);
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
        UIManager.instance.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        UIManager.instance.inputPanel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        StartCoroutine(FirstAttackMove(player));
        yield return StartCoroutine(FirstAttackMove(enemy));
        var attackCount = Mathf.Max(player.attackRequest.Count, enemy.attackRequest.Count);
        for (int i = 0; i < attackCount; i++)
        {
            // yield return new WaitForSeconds(skill.animation.length + 0.1f);

            if (lastSign == 0)
            {
                UIManager.instance.camRotZ = Random.Range(-20, 20);
                lastSign = (int)Mathf.Sign(UIManager.instance.camRotZ);
            }
            else
            {
                UIManager.instance.camRotZ = -lastSign * Random.Range(5, 20);
                lastSign *= -1;
            }

            float waitTime = Mathf.Max(AttackInit(player), AttackInit(enemy));
            player.BuffSetting(); enemy.BuffSetting();
            AttackStart(player); AttackStart(enemy);
            player.curSkill.effect?.End(player, player.target);
            enemy.curSkill.effect?.End(enemy, enemy.target);
            BuffClear(player); BuffClear(enemy);
            yield return new WaitForSeconds(waitTime);

            if (player.hp <= 0 || enemy.hp <= 0)
            {
                GameEnd();
                yield break;
            }
        }

        yield return new WaitForSeconds(0.5f);
        UIManager.instance.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);

        UIManager.instance.camRotZ = 0;
        player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        yield return enemy.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        UIManager.instance.inputPanel.rectTransform.sizeDelta = new Vector2(0, 250);
        isAttack = false;
        UIManager.instance.ActiveBtn(true);
        TurnEnd();
    }

    float AttackInit(Unit unit)
    {
        if (unit.attackRequest.Count <= 0) { unit.SkillInit(unit.nullSkill); return 0; }
        var skill = unit.SkillChange();
        unit.SkillInit(skill);
        print(skill.animation.name);

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

    void BuffClear(Unit unit)
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