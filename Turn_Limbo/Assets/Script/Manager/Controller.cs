using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class Skill
{
    public int index;
    public string skillName;
    public int[] cost;
    public int[] minDamage;
    public int[] maxDamage;
    public int level;
    public int attackCount;
    public int keyIndex;
    public Skill_Base effect;
    public Sprite icon;
    public string animationName;
    public Unit.ActionType actionType;
    public string explain;
    public PropertyType propertyType;
}

public class Controller : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    public Vector3 movePos;
    public SpriteRenderer bg;
    public Image keyHoldImage;
    public List<Skill_Base> skills = new();
    public List<Skill> inputLists = new();
    Queue<Dialogue> dialogueBox = new();
    DataManager data;
    public Unit talkUnit;

    public float gameCurTimeCount;
    public float keyHoldTime;
    public int useAbleCoin;
    public int useTurnCount;

    public bool isGame;
    public bool isTab;
    public bool isAttack;
    public bool isTimeSlowEffect;
    public bool isDialogue;
    public bool isSkillExplain;

    private int lastSign;
    private int spawnCount = 0;

    public VolumeProfile volume;
    [HideInInspector] public ChromaticAberration glitch;
    [HideInInspector] public DepthOfField depth;
    [HideInInspector] public ColorAdjustments color;

    [SerializeField] protected float AnimTime;

    public AudioClip hitSound;
    public AudioClip BGM;
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
        SetStage();
        TurnReset();
        data = DataManager.instance;
        UIManager.instance.SetExplain(false);
        player.hitSound = hitSound;
        enemy.hitSound = hitSound;
        player.dmgDelayTime = AnimTime;
        enemy.dmgDelayTime = AnimTime;

        depth.focalLength.value = 1;
        color.postExposure.value = 0;
        color.saturation.value = 0;

        useTurnCount = 1;
        if (BGM != null) SoundManager.instance.SetAudio(BGM, true);

    }
    public void SetStage()
    {
        // var enemy = Instantiate(DataManager.instance.SpawnData[ReadSpreadSheet.instance.curStageID - 2].enemies[spawnCount],new Vector3(5,-0.5f, 0), Quaternion.identity);
        // enemy.target = player;
        // enemy.unitUI = UIManager.instance.unitUI[1];
        // spawnCount++;
        var map = Instantiate(DataManager.instance.loadData.SpawnData[ReadSpreadSheet.instance.curStageID].maps);
        bg = map;
    }
    public void TurnReset()
    {
        gameCurTimeCount = 10;

        useAbleCoin += player.addCoin;
        player.TurnInit();
        enemy.TurnInit();
        UIManager.instance.ChangeCoinSkillImg();

        foreach (var n in player.usedBuff) ImageAnim(player, n.insertImage);
        player.usedBuff.Clear();
        foreach (var n in enemy.usedBuff) ImageAnim(enemy, n.insertImage);
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
            AddRequest(enemy, d.SkillList[enemy.skillInfo.SelectIndex[enemy.skillCurCount % enemy.skillInfo.SelectIndex.Count]]);
            enemy.skillCurCount++;
        }
    }
    void Update()
    {
        if (!data.readEnd) return;
        else if (data.curStageDialogBox.Count != 0)
        {
            StartCoroutine(StartDialogue(data.curStageDialogBox));
            data.curStageDialogBox = null;
        }

        if (isDialogue && Input.GetMouseButtonDown(0))
        {
            Dialogue();
        }

        UIUpdate(player);
        UIUpdate(enemy);
        int blurValue;
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
            UIManager.instance.SelectEnemyImage(false);
            UseAttack();
        }
        bool isSpace = Input.GetKey(KeyCode.Space) && isAttack;
        isTab = Input.GetKey(KeyCode.Tab) && !isAttack;
        float timeScale;

        if (isAttack)
        {
            timeScale = isSpace ? 0.4f : isTimeSlowEffect ? 0.15f : 1;
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

        character.unitUI.requestBuffParent.anchoredPosition
        = !isAttack ? ui.cam.WorldToScreenPoint(character.transform.position
        + new Vector3((3 - (5 - ui.cam.orthographicSize)) * (character.isLeft ? 1 : -1), 4))
        : new Vector3(ui.cam.WorldToScreenPoint(character.transform.position).x, 1000);
        character.unitUI.requestUIParent.anchoredPosition
        = Vector3.Lerp(character.unitUI.requestUIParent.anchoredPosition, requestPos, 0.05f);

        float scale = 0;
        if (isAttack) scale = 1.5f;
        else scale = 1 + (5 - ui.cam.orthographicSize) * 0.2f;
        character.unitUI.requestUIParent.localScale = Vector3.one * scale;
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
        if (isAttack) return;

        var newSkill = target.ConvertRequest(addSkill);
        newSkill.insertImage = UIManager.instance.AddIcon(newSkill.icon, target.unitUI.requestUIParent);
        target.attackRequest.Add(newSkill);
    }
    private void CheckInput()
    {
        if (useAbleCoin <= 0) return;
        var ui = UIManager.instance;
        if (isTab) return;
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
        data.readEnd = false;
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
        var ui = UIManager.instance;
        isAttack = true;
        ui.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        ui.inputPanel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        StartCoroutine(FirstAttackMove(player));
        yield return StartCoroutine(FirstAttackMove(enemy));
        var attackCount = Mathf.Max(player.attackRequest.Count, enemy.attackRequest.Count);
        Unit[] units = { player, enemy };
        for (int i = 0; i < attackCount; i++)
        {
            while (Vector3.Distance(player.transform.position, enemy.transform.position) >= 5)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, enemy.transform.position, Time.deltaTime * 15f);
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, Time.deltaTime * 15f);
                yield return null;
                //print(Vector3.Distance(player.transform.position, enemy.transform.position));
            }
            for (int j = 0; j < units.Length; j++)
            {
                if (units[j].attackRequest.Count < i + 1)
                {
                    units[j].InitCurSkillDamage(0, 0, 1);
                    LogView.instance.curDmg[j] = 0;
                }
            }

            if (ui.isCamRotate)
            {
                if (lastSign == 0)
                {
                    ui.camRotZ = Random.Range(-20, 20);
                    lastSign = (int)Mathf.Sign(ui.camRotZ);
                }
                else
                {
                    ui.camRotZ = -lastSign * Random.Range(5, 20);
                    lastSign *= -1;
                }
            }

            float waitTime = Mathf.Max(AttackInit(player), AttackInit(enemy));
            player.UseBuff(BuffTiming.turnStart); enemy.UseBuff(BuffTiming.turnStart);
            AttackStart(player); AttackStart(enemy);
            for (int j = 0; j < units.Length; j++)
            {
                LogView.instance.curSkills[j] = units[j].curSkill;
            }
            player.curSkill.effect?.End(player, player.target);
            enemy.curSkill.effect?.End(enemy, enemy.target);
            BuffClear(player); BuffClear(enemy);
            yield return new WaitForSeconds(waitTime + 0.01f);
            LogView.instance.AddLogs(player.Uniticon, enemy.Uniticon);

            if (talkUnit.isDialogue)
            {
                StartCoroutine(StartDialogue(data.hpDialogBox.Dequeue()));
                data.InitUnit(talkUnit);
                yield break;
            }

            if (player.hp <= 0 || enemy.hp <= 0)
            {
                GameEnd();
                yield break;
            }
        }

        yield return new WaitForSeconds(0.5f);
        ui.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);

        ui.camRotZ = 0;
        player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        yield return enemy.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        useTurnCount++;
        ui.inputPanel.rectTransform.sizeDelta = new Vector2(0, 250);
        isAttack = false;
        ui.ActiveBtn(true);
        TurnEnd();
    }

    float AttackInit(Unit unit)
    {
        if (unit.attackRequest.Count <= 0) { unit.SkillInit(unit.nullSkill); return 0; }
        var skill = unit.SkillChange();
        unit.SkillInit(skill);

        if(skill.animation == null) Debug.Log($"!!!!!!!!!!!! {skill.skillName}");
        return skill.animation.length;
    }

    void AttackStart(Unit unit)
    {
        var skill = unit.curSkill;
        if (unit.curSkill.actionType == Unit.ActionType.none) { return; }
        unit.InitCurSkillDamage(skill.minDamage[unit.skillInfo.holdSkills[skill.index].level],
            skill.maxDamage[unit.skillInfo.holdSkills[skill.index].level], skill.attackCount);

        unit.curSkill.effect?.Setting(unit, unit.target);
        unit.anim.Play(skill.animation.name);
        IconAnim(unit, skill.insertImage);
    }

    void BuffClear(Unit unit)
    {
        unit.curBuff = unit.ClearBuffList(unit.curBuff);

        foreach (var n in unit.usedBuff)
        {
            //Debug.Log($"{unit.name} {n.curBuff} {n.insertImage == null}");
            ImageAnim(unit, n.insertImage);
        }

        unit.usedBuff.Clear();
    }

    void ImageAnim(Unit unit, Image insertImage)
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
    void IconAnim(Unit unit, Icon insertImage)
    {
        unit.iconAnim = DOTween.Sequence();

        unit.iconAnim.Append(insertImage.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuint));
        unit.iconAnim.Append(insertImage.transform.DOScale(0, 0.2f).SetEase(Ease.OutQuint));
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

    void Dialogue()
    {
        if (dialogueBox.Count == 0 && !DialogueManager.instance.isTyping || DialogueManager.instance.isEnd) StartCoroutine(EndDialogue());
        else if (!DialogueManager.instance.isTyping)
        {
            if (!DialogueManager.instance.panelState)
            {
                DialogueManager.instance.InputDialogue(dialogueBox.Dequeue());
                StartCoroutine(DialogueManager.instance.TypingText());
            }
        }
        else StartCoroutine(DialogueManager.instance.TypingText());
    }

    IEnumerator StartDialogue(Queue<Dialogue> curDialogueBox)
    {
        isAttack = true;
        isDialogue = true;
        dialogueBox = curDialogueBox;
        DialogueManager.instance.OnOffDialogue(isAttack);
        player.unitUI.HideUI(false);
        enemy.unitUI.HideUI(false);
        // StartCoroutine(FirstDialogueMove(player));
        // yield return StartCoroutine(FirstDialogueMove(enemy));
        yield return null;
        DialogueManager.instance.InputDialogue(dialogueBox.Dequeue());
        StartCoroutine(DialogueManager.instance.TypingText());
    }

    IEnumerator EndDialogue()
    {
        isAttack = false;
        DialogueManager.instance.OnOffDialogue(isAttack);
        player.unitUI.HideUI(true);
        enemy.unitUI.HideUI(true);
        dialogueBox.Clear();
        StartCoroutine(EndDialogueMove(player));
        yield return EndDialogueMove(enemy);
    }

    // //Dialogue position
    // IEnumerator FirstDialogueMove(Unit unit)
    // {
    //     movePos = Vector3.Lerp(unit.transform.position, unit.target.transform.position, 0.5f);

    //     yield return unit.transform.DOMoveX(movePos.x - (2 * (unit.isLeft ? 1 : -1)), 0.5f)
    //     .SetEase(Ease.OutCubic).WaitForCompletion();
    // }

    IEnumerator EndDialogueMove(Unit unit)
    {
        yield return unit.transform.DOMoveX(-3.5f * (unit.isLeft ? 1 : -1), 0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
    }
}