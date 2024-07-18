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
    public int index;
    public int skillTier;
    public string skillName;
    public int[] cost;
    public int[] minDamage;
    public int[] maxDamage;
    public int level;
    public int attackCount;
    public int keyIndex;
    public int sale;
    public bool isOnlyEnemy;
    public SkillScript effect;
    public Sprite icon;
    public string animationName;
    public ActionType actionType;
    public string effect_desc;
    public string skill_desc;
    public PropertyType propertyType;
}

public class Controller : MonoBehaviour, IInitObserver
{
    public Player player;
    public Enemy enemy;
    public Vector3 movePos;
    public SpriteRenderer bg;
    public Image keyHoldImage;
    public List<SkillScript> skills = new();
    public List<Skill> inputLists = new();

    [Header("dialog")]
    Queue<Dialogue> dialogueBox = new();
    DataManager data;
    public Unit talkUnit;

    public float gameCurTimeCount;
    public float keyHoldTime;
    public int useAbleCoin;
    [HideInInspector]
    public int useTurnCount;
    public int useTotalTurnCount;
    public int enemyKillCount;

    public bool isGame;
    public bool isTab;
    public bool isChain;
    public bool isAttack;
    public bool isBoss;
    public bool isTimeSlowEffect;
    public bool isDialogue;
    public bool isSkillExplain;
    public bool isEnemyRandomSkillCount;
    public bool isEnemyRandomSkillIndex;

    private int lastSign;
    public int spawnCount = 0;

    [Header("Post Processing")]
    public VolumeProfile volume;
    [HideInInspector] public ChromaticAberration glitch;
    [HideInInspector] public DepthOfField depth;
    [HideInInspector] public ColorAdjustments color;

    [SerializeField] protected float AnimTime;

    [Header("Sound")]
    public AudioClip[] hitSound;
    public AudioClip CritSound;
    public AudioClip[] addSkillSound;

    public Dictionary<int, List<Skill>> inputs = new();
    private readonly KeyCode[] KEY_CODE = { KeyCode.Q, KeyCode.W, KeyCode.E };

    [Header("option")]
    [SerializeField] IngamePause pause;

    public int Priority => 1;

    public void Init()
    {
        volume.TryGet(out glitch);
        volume.TryGet(out depth);
        volume.TryGet(out color);
        data = DataManager.instance;

        player.hitSound = hitSound;
        enemy.hitSound = hitSound;
        player.dmgDelayTime = AnimTime;
        enemy.dmgDelayTime = AnimTime;

        depth.focalLength.value = 1;
        color.postExposure.value = 0;
        color.saturation.value = 0;
    }
    public void SetStage()
    {
        var d = DataManager.instance;
        var r = RoglikeManager.instance;
        SpriteRenderer map;
        var spawnDataIndex = d.curMode == RoglikeManager.Modes.stage ? d.curStageID : r.rogStageIndex;
        map = Instantiate(d.loadData.SpawnData[spawnDataIndex].maps);

        if (d.curMode == RoglikeManager.Modes.rogLike) r.ShowStagePanel();
        else
        {
            SpawnEnemy();
            InitEnemy();
        }

        bg = map;
        d.plusStats.AddStats(player);
        d.plusStats.AddBuff(player);
    }
    public void SpawnEnemy()
    {
        var d = DataManager.instance;
        var r = RoglikeManager.instance;
        if (d.curMode == RoglikeManager.Modes.stage)
        {
            enemy = Instantiate(d.loadData.SpawnData[d.curStageID].enemies[spawnCount], new Vector3(12, -0.5f, 0), Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(r.roglikeData.stageDatas[r.rogStageIndex].spawnEnemyList[spawnCount], new Vector3(12, -0.5f, 0), Quaternion.identity);
        }

        enemy.hitSound = hitSound;
        enemy.dmgDelayTime = AnimTime;

        enemy.target = player;
        player.target = enemy;
        enemy.unitUI = UIManager.instance.unitUI[1];
        enemy.transform.DOMoveX(5, 0.5f);
        spawnCount++;
        GiveEnemySkill();
    }
    public void GiveEnemySkill()
    {
        var skillList = enemy.skillInfo;
        for (int i = 0; i < skillList.selectIndex.Count; i++)
        {
            var newSkill = new HoldSkills()
            {
                holdIndex = i,
                level = 0
            };
            skillList.holdSkills.TryAdd(skillList.selectIndex[i], newSkill);
        }
    }
    public void TurnReset()
    {
        gameCurTimeCount = 10;

        useAbleCoin += player.nextTurnAddCoin;
        useAbleCoin = Mathf.Clamp(useAbleCoin, 0, 10);
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
        var d = DataManager.instance;
        var r = RoglikeManager.instance;
        if (enemy == null)
        {
            if (d.curMode == RoglikeManager.Modes.stage)
            {
                if (spawnCount < DataManager.instance.loadData.SpawnData[DataManager.instance.curStageID].enemies.Count)
                {
                    SpawnEnemy();
                    InitEnemy();
                    TurnReset();
                }
                else
                {
                    UIManager.instance.isPause = true;
                    Time.timeScale = 0;
                }
            }
            else
            {
                if (spawnCount < r.roglikeData.stageDatas[r.rogStageIndex].spawnEnemyList.Length)
                {
                    r.GetItemRandom();
                    SpawnEnemy();
                    InitEnemy();
                    TurnReset();
                }
                else
                {
                    r.GetItemRandom();
                }
            }
        }
        else
        {
            InitEnemy();
            TurnReset();
        }
    }
    public void InitEnemy()
    {
        var d = DataManager.instance;
        var count = (useTurnCount - 1) % enemy.skillInfo.turnActCounts.Count;
        var coinCount = d.curMode == RoglikeManager.Modes.rogLike
        ? Random.Range(enemy.requestMinCount, enemy.requestMaxCount + 1)
        : enemy.skillInfo.turnActCounts[count];

        // print(coinCount);
        // print($"{useTurnCount - 1} % {enemy.skillInfo.turnActCounts.Count} = {count}");
        // print(isEnemyRandomSkillCount);
        for (int i = 0; i < coinCount; i++)
        {
            var addIndex = d.curMode == RoglikeManager.Modes.stage ? enemy.skillCurCount % enemy.skillInfo.selectIndex.Count : Random.Range(0, enemy.skillInfo.selectIndex.Count);
            AddRequest(enemy, d.loadData.SkillList[enemy.skillInfo.selectIndex[addIndex]]);
            enemy.skillCurCount++;
        }
    }
    void Update()
    {
        if (UIManager.instance.isPause || RoglikeManager.instance.isEvent) return;
        if (!data.readEnd) return;
        else if (data.stageDialogBox.Count != 0 && !isDialogue)
        {
            Debug.Log($"stage : {data.loadData.stageDialogBox.Count} / hp : {data.loadData.hpDialogBox.Count} / id : {data.curStageID} / data_stage : {data.stageDialogBox.Count} / data_hp : {data.hpDialogBox.Count}");
            StartCoroutine(StartDialogue(data.stageDialogBox));
        }

        if (isDialogue && Input.GetMouseButtonDown(0)) Dialogue();

        UIUpdate(player);
        if (enemy != null) UIUpdate(enemy);
        int blurValue;
        if (isAttack && isGame) blurValue = 500;
        else
        {
            blurValue = 1;
            gameCurTimeCount -= Time.deltaTime;
            if (gameCurTimeCount <= 0) UseAttack();
        }
        UIUpdate(player);
        if (enemy != null) UIUpdate(enemy);
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
        Color bgColor = isSpace || isTab || isChain ? new Color(0.6f, 0.6f, 0.6f, 1) : Color.white;
        bg.color = bg.color.MoveToward(bgColor, Time.deltaTime * 5f);
        //if(isChain) UIManager.instance
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
            UIManager.instance.NextImage(i, inputs[i][0].icon, inputs[i][1].icon, inputs[i][0].cost[inputs[i][0].level]);
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
                    RequestKeyUp(i);
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
                keyHoldImage.transform.position = ui.keys[i].transform.position;
                if (keyHoldTime > 1f)
                {
                    isSkillExplain = true;
                    keyHoldTime = 0;
                    if (player.skillInfo.holdSkills.TryGetValue(inputs[i][0].index - 1, out var holdSkill))
                        ui.SetExplain(true, inputs[i][0], ui.keys[i].rectTransform.anchoredPosition, holdSkill.level);
                }
                keyHoldImage.fillAmount = Mathf.Clamp(keyHoldTime - 0.3f, 0, 10) / 0.5f;
            }
        }
    }
    public void RequestKeyUp(int i)
    {
        if (inputs[i][0].cost[0] > useAbleCoin) return;
        var ui = UIManager.instance;
        var input = inputs[i];
        AddRequest(player, input[0]);
        useAbleCoin -= input[0].cost[input[0].level];
        SwapSkills(input);
        ui.ChangeCoinSkillImg();
        ui.NextImage(i, input[0].icon, input[1].icon, input[0].cost[input[0].level]);
        SoundManager.instance.SetAudio(addSkillSound[Random.Range(0, addSkillSound.Length)], false);
    }
    public void PhaseEnd()
    {
        if (player.hp <= 0) GameOver();
        else StartCoroutine(KillEnemy());
    }
    public IEnumerator KillEnemy()
    {
        var d = DataManager.instance;
        var r = RoglikeManager.instance;
        var deadEnemy = enemy;
        yield return deadEnemy.transform.DOMoveX(12, 0.5f).OnComplete(() => Destroy(deadEnemy.gameObject)).WaitForCompletion();

        foreach (Transform child in enemy.unitUI.requestUIParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in player.unitUI.requestUIParent)
        {
            Destroy(child.gameObject);
        }
        enemy.target = null;
        enemy.unitUI = null;
        player.target = null;
        enemy = null;
        useTurnCount = 0;
        enemyKillCount++;

        if (d.curMode == RoglikeManager.Modes.stage)
        {
            if (spawnCount >= d.loadData.SpawnData[d.curStageID].enemies.Count)
                GameClear();
        }
        else
        {
            if (spawnCount >= r.roglikeData.stageDatas[r.rogStageIndex].spawnEnemyList.Length)
            {
                if (r.roglikeData.stageDatas[r.rogStageIndex].spawnBoss == null || isBoss)
                {
                    r.ShowStagePanel();
                    d.saveData.money += r.roglikeData.stageDatas[r.rogStageIndex].clearGetMoney;
                }
                else
                {
                    isBoss = true;
                    enemy = Instantiate(r.roglikeData.stageDatas[r.rogStageIndex].spawnBoss, new Vector3(12, -0.5f, 0), Quaternion.identity);

                    enemy.hitSound = hitSound;
                    enemy.dmgDelayTime = AnimTime;

                    enemy.target = player;
                    player.target = enemy;
                    enemy.unitUI = UIManager.instance.unitUI[1];
                    enemy.transform.DOMoveX(5, 0.5f);
                    spawnCount++;
                    GiveEnemySkill();
                }
            }
        }


    }
    public void GameOver()
    {
        UIManager.instance.isPause = true;
        Time.timeScale = 0;
        UIManager.instance.SetGameEndUI(false);
    }
    public void GameClear()
    {
        UIManager.instance.SetGameEndUI(true);
        print("Game Claer");
    }

    public void UseAttack()
    {
        if (isDialogue || isAttack || RoglikeManager.instance.isEvent || UIManager.instance.isPause) return;
        isAttack = true;
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
        ui.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        ui.inputPanel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        var attackCount = FirstAttackCheck(player, enemy);
        StartCoroutine(FirstAttackMove(player));
        yield return StartCoroutine(FirstAttackMove(enemy));
        Unit[] units = { player, enemy };
        ui.attackView.OnOff(true);
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
            ui.attackView.Reset();
            float playerDelay = AttackInit(player);
            float enemyDelay = AttackInit(enemy);
            float waitTime = Mathf.Max(playerDelay * player.curSkill.attackCount, enemyDelay * enemy.curSkill.attackCount);

            player.UseBuff(BuffTiming.TurnStart); enemy.UseBuff(BuffTiming.TurnStart);
            float checkTime = AttackCheck(player, enemy);
            waitTime = checkTime == 0 ? waitTime : checkTime;
            StartCoroutine(AttackStart(player)); StartCoroutine(AttackStart(enemy));

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
                PhaseEnd();
                break;
            }
        }

        yield return new WaitForSeconds(0.5f);
        ui.attackView.OnOff(false);
        ui.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);
        player.anim.Play("Idle"); enemy?.anim.Play("Idle");

        ui.camRotZ = 0;
        enemy?.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        yield return player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        .SetEase(Ease.InOutSine).WaitForCompletion();
        useTurnCount++;
        useTotalTurnCount++;
        ui.inputPanel.rectTransform.sizeDelta = new Vector2(0, 352);
        isAttack = false;
        ui.ActiveBtn(true);
        player.UseBuff(BuffTiming.BattleEnd); enemy?.UseBuff(BuffTiming.BattleEnd);
        TurnEnd();
    }

    int FirstAttackCheck(Unit player, Unit enemy){
        bool findChain = false;

        foreach (var n in player.attackRequest){if(n.actionType == ActionType.Chain) findChain = true;}
        foreach (var n in enemy.attackRequest){if(n.actionType == ActionType.Chain) findChain = true;}

        var bigtemp = player.attackRequest.Count > enemy.attackRequest.Count ? player.attackRequest : enemy.attackRequest;
        var smalltemp = player.attackRequest.Count > enemy.attackRequest.Count ? enemy.attackRequest : player.attackRequest;
        _ = new RequestSkill();

        if (smalltemp.Count == 0 || !findChain || (bigtemp.Count == smalltemp.Count)) {
            //Debug.Log($"Return / {smalltemp.Count} {findChain}");
            return bigtemp.Count;
        }

        RequestSkill skilltemp = smalltemp.Last();
        smalltemp.RemoveAt(smalltemp.Count -1);
        while(true){
            smalltemp.Add(player.nullSkill);
            if(bigtemp.Count - 1 == smalltemp.Count) break;
        }
        smalltemp.Add(skilltemp);
        //Debug.Log($"Setting / p : {player.attackRequest.Count} / e : {enemy.attackRequest.Count}");

        return bigtemp.Count;
    }

    float AttackInit(Unit unit)
    {
        if (unit.attackRequest.Count <= 0) { unit.SkillInit(unit.nullSkill); return 0; }

        var skill = unit.SkillChange();
        unit.SkillInit(skill);

        return skill.animation != null ? skill.animation.length : 0;
    }

    float AttackCheck(Unit player, Unit enemy)
    {
        var p = player.curSkill.actionType;
        var e = enemy.curSkill.actionType;

        bool isChain = false;

        if (p == ActionType.Chain)
        {
            isChain = true;
            enemy.curSkill.actionType = e == ActionType.none ? ActionType.none :
            e == ActionType.Chain ? ActionType.Chain : ActionType.Change;
        }
        else if (e == ActionType.Chain)
        {
            isChain = true;
            player.curSkill.actionType = p == ActionType.none ? ActionType.none :
            p == ActionType.Chain ? ActionType.Chain : ActionType.Change;
        }

        if (isChain)
        {
            this.isChain = true;

            if (player.attackRequest.Count == 0) return player.curSkill.animation == null ? 0.4f : player.curSkill.animation.length;
            else if (enemy.attackRequest.Count == 0) return enemy.curSkill.animation == null ? 0.4f : enemy.curSkill.animation.length;
            else return 0.4f;
        }
        else
        {
            this.isChain = false;
            return 0;
        }
    }

    IEnumerator AttackStart(Unit unit)
    {
        var d = DataManager.instance;
        var ui = UIManager.instance;
        var skill = unit.curSkill;
        //print($"{unit.name} : {unit.curAttackCount},{unit.curSkill.index}");
        if (unit.curSkill.actionType == ActionType.none) yield break;
        StartCoroutine(IconAnim(skill.insertImage, skill.animation.length * skill.attackCount));

        if (unit.curSkill.actionType == ActionType.Chain || unit.curSkill.actionType == ActionType.Change)
        {
            unit.isChain = true;
            unit.chainDamage += skill.minDamage[d.loadData.SkillList[skill.index - 1].level];
            unit.chainCount++;
            if (unit.curSkill.actionType == ActionType.Chain)
            {
                unit.chainName.Add(skill.skillName);
                unit.curSkill.effect?.Setting(unit, unit.target);
            }
        }
        else
        {
            unit.InitCurSkillDamage(skill.minDamage[d.loadData.SkillList[skill.index - 1].level],
            skill.maxDamage[d.loadData.SkillList[skill.index - 1].level], skill.attackCount);

            unit.curSkill.effect?.Setting(unit, unit.target);
            ChainAttack();
            for (int i = 0; i < skill.attackCount; i++)
            {
                unit.anim.Play(skill.propertyType.ToString(), -1, 0f);
                yield return new WaitForSeconds(skill.animation.length);
            }
        }

        void ChainAttack()
        {
            if (unit.isChain)
            {
                string temp = "";
                foreach (var n in unit.chainName) { temp += n + "-"; }
                StartCoroutine(ui.attackView.ChainAttack(unit.isLeft, temp + unit.curSkill.skillName));
                Debug.Log(temp + unit.curSkill.skillName);
                unit.chainName.Clear();
                unit.isChain = false;
            }
        }
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
    IEnumerator IconAnim(Icon insertImage, float waitTime)
    {
        yield return insertImage.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutQuint).WaitForCompletion();
        yield return new WaitForSeconds(Mathf.Clamp(waitTime - 0.4f, 0, 5));
        yield return insertImage.transform.DOScale(0, 0.2f).SetEase(Ease.OutQuint).WaitForCompletion();
        Destroy(insertImage.gameObject);
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
        isDialogue = false;
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