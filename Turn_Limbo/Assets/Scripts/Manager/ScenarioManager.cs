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

    //singleton
    private UIManager uiManager => UIManager.instance;

    [SerializeField] Image panel;

    [Header("Dialogue")]
    [SerializeField] Image barSize;
    [SerializeField] Image nameLeftBar;
    [SerializeField] Image nameRightBar;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI leftNameText;
    [SerializeField] TextMeshProUGUI leftJobText;
    [SerializeField] TextMeshProUGUI rightNameText;
    [SerializeField] TextMeshProUGUI rightJobText;
    [SerializeField] GameObject focusUI;
    [SerializeField] float typingTime;

    [HideInInspector] public bool isTyping;
    [HideInInspector] public bool panelState;
    [HideInInspector] public bool isEnd;
    bool isSkip;
    bool isNameHide;
    // Action<string> curEvent;

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
        leftNameText.text = null;
        text.text = null;
        uiManager.timerBG.gameObject.SetActive(false);
        focusUI.SetActive(true);
        uiManager.inputPanel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        barSize.rectTransform.DOSizeDelta(new Vector2(0, 275), 0.5f);

        for (int dialogueIndex = 0; dialogueIndex < scenario.dialogues.Length; dialogueIndex++)
        {
            //dialogue initialize
            panel.gameObject.SetActive(false);

            var dialogue = scenario.dialogues[dialogueIndex];
            SetNameBoxPos(dialogue);
            SetCamPos(dialogue);

            leftNameText.text = dialogue.name;
            leftJobText.text = dialogue.job;
            rightNameText.text = dialogue.job;
            rightJobText.text = dialogue.job;

            //text typing
            isTyping = true;
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
            isTyping = false;
            isSkip = false;

            ExecuteDialogueEvent(dialogue);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        //hide
        text.text = null;
        uiManager.camPlusPos = Vector3.zero;
        uiManager.timerBG.gameObject.SetActive(true);
        nameLeftBar.rectTransform.DOLocalMoveX(-1410, 0.5f);
        nameRightBar.rectTransform.DOLocalMoveX(1410, 0.5f);
        focusUI.SetActive(false);
        uiManager.inputPanel.rectTransform.DOSizeDelta(new(0, 250), 0.5f);
        barSize.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);

        yield return null;
    }

    private void SetNameBoxPos(DialogueInfo dialogue)
    {
        switch (dialogue.namePos)
        {
            case NamePos.Left:
                nameLeftBar.rectTransform.DOLocalMoveX(-960, 0.5f);
                nameRightBar.rectTransform.DOLocalMoveX(1410, 0.5f);
                isNameHide = false; break;

            case NamePos.Right:
                nameLeftBar.rectTransform.DOLocalMoveX(-1410, 0.5f);
                nameRightBar.rectTransform.DOLocalMoveX(960, 0.5f);
                isNameHide = false; break;

            case NamePos.Hide:
                if (isNameHide) break;
                else
                {
                    nameLeftBar.rectTransform.DOLocalMoveX(-1410, 0.5f);
                    nameRightBar.rectTransform.DOLocalMoveX(1410, 0.5f);
                    isNameHide = true;
                }
                break;
        }
    }

    private void SetCamPos(DialogueInfo dialogue)
    {
        switch (dialogue.camPos)
        {
            case CamPos.LeftEnd: uiManager.camPlusPos = new Vector3(-3, -1); break;
            case CamPos.Left: uiManager.camPlusPos = new Vector3(-1, -1); break;
            case CamPos.Middle: uiManager.camPlusPos = new Vector3(0, -1); break;
            case CamPos.Right: uiManager.camPlusPos = new Vector3(1, -1); break;
            case CamPos.RightEnd: uiManager.camPlusPos = new Vector3(3, -1); break;
        }
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
        for(int i = 0; i < dialogue.eventValue.Length; i++)
        {
            if(dialogue.eventValue[i] == '(')
            {
                command = dialogue.eventValue[0..i];
                i++;
                valueStartIndex = i;
            }
            
            if(dialogue.eventValue[i] == ')')
                value = dialogue.eventValue[valueStartIndex..i];
        }

        switch(command)
        {
            case "Tutorial":
                panel.sprite = Resources.Load<Sprite>($"Panel/{value}");
                panel.gameObject.SetActive(true);
                break;
        }
    }
}
