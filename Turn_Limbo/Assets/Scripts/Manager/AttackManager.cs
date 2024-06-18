using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private DataManager dataManager => DataManager.instance;

    private readonly KeyCode[] KEY_CODE = { KeyCode.Q, KeyCode.W, KeyCode.E };

    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    public int act;
    public bool isAttack;
    public List<SkillInfo> usedSkills = new();
    public List<SkillInfo>[] skillQueues = new List<SkillInfo>[3];
    public Dictionary<string, Skill_Base> skillInstanceTables = new();

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
            skillQueues[i] = new();
    }

    private void Start()
    {
        for (int i = 0; i < dataManager.saveData.deckSkills.Count; i++)
        {
            var skill = dataManager.loadData.skillInfos[i];
            skillQueues[skill.keyIndex].Add(skill);

            var script = "Skill_" + skill.script;
            if (!skillInstanceTables.ContainsKey(script))
                skillInstanceTables.Add(script, Activator.CreateInstance(Type.GetType(script)) as Skill_Base);
        }
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetKeyDown(KEY_CODE[i]))
            {
                var useSkill = skillQueues[i][0];
                var useAct = useSkill.act[dataManager.saveData.skillLevels[useSkill.index]];

                if (act >= useAct)
                {
                    usedSkills.Add(useSkill);
                    skillQueues[i].RemoveAt(0);
                    skillQueues[i].Add(useSkill);

                    act -= useAct;
                }
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttack = true;
        // ui.cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
        // ui.inputPanel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);

        // StartCoroutine(FirstAttackMove(player));
        // yield return StartCoroutine(FirstAttackMove(enemy));

        for (int i = 0; i < Mathf.Max(player.attackRequest.Count, enemy.attackRequest.Count); i++)
        {
            while (Vector3.Distance(player.transform.position, enemy.transform.position) >= 5)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, enemy.transform.position, Time.deltaTime * 15f);
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, Time.deltaTime * 15f);
                yield return null;
                //print(Vector3.Distance(player.transform.position, enemy.transform.position));
            }

            // for (int j = 0; j < units.Length; j++)
            // {
            //     if (units[j].attackRequest.Count < i + 1)
            //     {
            //         units[j].InitCurSkillDamage(0, 0, 1);
            //         LogView.instance.curDmg[j] = 0;
            //     }
            // }

            // if (ui.isCamRotate)
            // {
            //     if (lastSign == 0)
            //     {
            //         ui.camRotZ = Random.Range(-20, 20);
            //         lastSign = (int)Mathf.Sign(ui.camRotZ);
            //     }
            //     else
            //     {
            //         ui.camRotZ = -lastSign * Random.Range(5, 20);
            //         lastSign *= -1;
            //     }
            // }
            // float waitTime = Mathf.Max(AttackInit(player) * player.curSkill.attackCount, AttackInit(enemy) * enemy.curSkill.attackCount);

            // player.UseBuff(BuffReduceTiming.turnStart); enemy.UseBuff(BuffReduceTiming.turnStart);
            // StartCoroutine(AttackStart(player)); StartCoroutine(AttackStart(enemy));

            // for (int j = 0; j < units.Length; j++)
            // {
            //     LogView.instance.curSkills[j] = units[j].curSkill;
            // }
            // player.curSkill.effect?.End(player, player.target);
            // enemy.curSkill.effect?.End(enemy, enemy.target);
            // BuffClear(player); BuffClear(enemy);
            // yield return new WaitForSeconds(waitTime + 0.01f);
            // LogView.instance.AddLogs(player.Uniticon, enemy.Uniticon);

            // if (talkUnit.isDialogue)
            // {
            //     // StartCoroutine(StartDialogue(dataManager.hpDialogBox.Dequeue()));
            //     yield break;
            // }

            // if (player.hp <= 0 || enemy.hp <= 0)
            // {
            //     GameEnd();
            //     yield break;
            // }
        }

        // yield return new WaitForSeconds(0.5f);
        // ui.cam.DOOrthoSize(5f, 0.5f).SetEase(Ease.OutCubic);
        // player.anim.Play("Idle"); enemy.anim.Play("Idle"); 

        // ui.camRotZ = 0;
        // player.transform.DOMoveX(-3.5f * (player.isLeft ? 1 : -1), 0.5f)
        // .SetEase(Ease.InOutSine).WaitForCompletion();
        // yield return enemy.transform.DOMoveX(-3.5f * (enemy.isLeft ? 1 : -1), 0.5f)
        // .SetEase(Ease.InOutSine).WaitForCompletion();
        // useTurnCount++;
        // ui.inputPanel.rectTransform.sizeDelta = new Vector2(0, 250);
        // isAttack = false;
        // ui.ActiveBtn(true);
        // TurnEnd();
    }
}