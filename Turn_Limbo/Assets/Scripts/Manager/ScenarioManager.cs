using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;

public class ScenarioManager : MonoBehaviour
{
    public enum NamePos
    {
        Left,
        Right,
        Hide
    }

    public enum CamPos
    {
        LeftEnd,
        Left,
        Middle,
        Right,
        RightEnd
    }

    public enum DialogueEvent
    {
        Null,
        Panel,
        Player_Damage,
        Enemy_Damage,
    }

    public static ScenarioManager instance { get; private set; }

    [SerializeField] Image panel;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject leftBox;
    [SerializeField] private GameObject rightBox;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI leftNameText;
    [SerializeField] private TextMeshProUGUI leftJobText;
    [SerializeField] private TextMeshProUGUI rightNameText;
    [SerializeField] private TextMeshProUGUI rightJobText;
    [SerializeField] private GameObject focusUI;
    [SerializeField] private float typingTime;

    bool isSkip;

    public void Skip()
    {
        isSkip = true;
    }

    private void Awake()
    {
        instance = this;
        text.text = null;
    }

    public void StartScenario(ScenarioInfo scenario)
    {
        StartCoroutine(ScenarioRoutine(scenario));
    }

    private IEnumerator ScenarioRoutine(ScenarioInfo scenario)
    {
        //scenario initialize
        dialogueUI.SetActive(true);
        focusUI.SetActive(true);

        for (int dialogueIndex = 0; dialogueIndex < scenario.dialogues.Length; dialogueIndex++)
        {
            //dialogue initialize
            panel.gameObject.SetActive(false);

            var dialogue = scenario.dialogues[dialogueIndex];

            SetCamPos(dialogue);
            SetNamePos(dialogue);

            //text typing
            text.text = dialogue.text;
            for (int curTextLength = 0; curTextLength < dialogue.text.Length; curTextLength++)
            {
                text.textInfo.characterCount = curTextLength;
                var curTypingTime = 0f;
                while (curTypingTime <= typingTime)
                {
                    curTypingTime += Time.deltaTime;
                    if (isSkip) break;
                    yield return null;
                }
                if (isSkip) break;
            }
            isSkip = false;

            ExecuteDialogueEvent(dialogue);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        //hide
        dialogueUI.SetActive(false);
        focusUI.SetActive(false);
        rightBox.SetActive(false);
        leftBox.SetActive(false);

        yield return null;
    }

    private void SetNamePos(DialogueInfo dialogue)
    {
        switch (dialogue.namePos)
        {
            case NamePos.Left:
                leftNameText.text = dialogue.name;
                leftJobText.text = dialogue.job;
                leftBox.SetActive(true);
                rightBox.SetActive(false);
                break;
            case NamePos.Right:
                rightNameText.text = dialogue.name;
                rightJobText.text = dialogue.job;
                rightBox.SetActive(true);
                leftBox.SetActive(false);
                break;
            case NamePos.Hide:
                
                break;
        }
    }

    private void SetCamPos(DialogueInfo dialogue)
    {
        // switch (dialogue.camPos)
        // {
        //     case CamPos.LeftEnd: uiManager.camPlusPos = new Vector3(-3, -1); break;
        //     case CamPos.Left: uiManager.camPlusPos = new Vector3(-1, -1); break;
        //     case CamPos.Middle: uiManager.camPlusPos = new Vector3(0, -1); break;
        //     case CamPos.Right: uiManager.camPlusPos = new Vector3(1, -1); break;
        //     case CamPos.RightEnd: uiManager.camPlusPos = new Vector3(3, -1); break;
        // }
    }

    // public void InputDialogue(DialogueInfo dialogue)
    // {
    //     switch (dialogue.curEvent)
    //     {
    //         // case CurEvent.Null: curEvent = null; break;
    //         // case CurEvent.Panel: curEvent = SettingPanel; break;
    //         // case CurEvent.Player_Damage: curEvent = controller.player.Damage; break;
    //         // case CurEvent.Enemy_Damage: curEvent = controller.enemy.Damage; break;
    //     }
    //     // eventValue = dialogue.eventValue;
    // }

    private void ExecuteDialogueEvent(DialogueInfo dialogue)
    {
        string command = null;
        string value = null;

        int valueStartIndex = 0;
        for (int i = 0; i < dialogue.eventValue.Length; i++)
        {
            if (dialogue.eventValue[i] == '(')
            {
                command = dialogue.eventValue[0..i];
                i++;
                valueStartIndex = i;
            }

            if (dialogue.eventValue[i] == ')')
                value = dialogue.eventValue[valueStartIndex..i];
        }

        switch (command)
        {
            case "Tutorial":
                panel.sprite = Resources.Load<Sprite>($"Panel/{value}");
                panel.gameObject.SetActive(true);
                break;
        }
    }
}
