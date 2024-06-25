using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;


[System.Serializable]
public class Dialogue
{
    public string name;
    public string job;
    public string text;
    public DialogueManager.NamePos namePos;
    public DialogueManager.CamPos camPos;
    public DialogueManager.CurEvent curEvent;
    public string eventValue;
    public int hpValue;
    //public Sprite icon;
    //effect
    //target
    //background
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
        text.text = null;
        nameText.text = null;
        jobText.text = null;
        waitTime = new WaitForSeconds(typingTime);
    }
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
    public enum CurEvent
    {
        Null,
        Panel,
        Player_Damage,
        Enemy_Damage,
    }

    [SerializeField] Image panel;

    [Header("Dialogue")]
    [SerializeField] Image barSize;
    [SerializeField] Image nameLeftBar;
    [SerializeField] Image nameRightBar;
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text jobText;
    [SerializeField] GameObject focusUI;
    private string tempText;
    [SerializeField] float typingTime;
    WaitForSeconds waitTime;
    [HideInInspector] public bool isTyping;
    [HideInInspector] public bool panelState;
    [HideInInspector] public bool isEnd;
    string eventValue;
    bool isSkip;
    [SerializeField] bool isPanel;
    bool isNameHide;
    Action<string> curEvent;

    public void OnOffDialogue(bool isOn)
    {
        if (isOn)
        {
            //cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
            nameText.text = null;
            text.text = null;
            UIManager.instance.timerBG.gameObject.SetActive(!isOn);
            focusUI.SetActive(true);
        }
        else
        {
            text.text = null;
            UIManager.instance.camPlusPos = Vector3.zero;
            UIManager.instance.timerBG.gameObject.SetActive(!isOn);
            nameLeftBar.rectTransform.DOLocalMoveX(-1410, 0.5f);
            nameRightBar.rectTransform.DOLocalMoveX(1410, 0.5f);
            focusUI.SetActive(false);
        }
        UIManager.instance.inputPanel.rectTransform.DOSizeDelta(isOn ? Vector2.zero : new(0, 352), 0.5f);
        barSize.rectTransform.DOSizeDelta(isOn ? new(0, 275) : Vector2.zero, 0.5f);
    }

    public void InputDialogue(Dialogue dialogue)
    {
        //Debug.Log("Input Dialogue");
        switch (dialogue.namePos)
        {
            case NamePos.Left:
                nameLeftBar.rectTransform.DOLocalMoveX(-960, 0.5f);
                nameRightBar.rectTransform.DOLocalMoveX(1410, 0.5f);
                nameText.transform.parent = nameLeftBar.transform;
                jobText.transform.parent = nameLeftBar.transform;
                isNameHide = false; break;

            case NamePos.Right:
                nameLeftBar.rectTransform.DOLocalMoveX(-1410, 0.5f);
                nameRightBar.rectTransform.DOLocalMoveX(960, 0.5f);
                nameText.transform.parent = nameRightBar.transform;
                jobText.transform.parent = nameRightBar.transform;
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

        switch (dialogue.camPos)
        {
            case CamPos.LeftEnd: UIManager.instance.camPlusPos = new Vector3(-3, -1); break;
            case CamPos.Left: UIManager.instance.camPlusPos = new Vector3(-1, -1); break;
            case CamPos.Middle: UIManager.instance.camPlusPos = new Vector3(0, -1); break;
            case CamPos.Right: UIManager.instance.camPlusPos = new Vector3(1, -1); break;
            case CamPos.RightEnd: UIManager.instance.camPlusPos = new Vector3(3, -1); break;
        }
        switch (dialogue.curEvent)
        {
            case CurEvent.Null: curEvent = null; break;
            case CurEvent.Panel: curEvent = SettingPanel; break;
                //case CurEvent.Player_Damage: curEvent = controller.player.Damage; break;
                //case CurEvent.Enemy_Damage: curEvent = controller.enemy.Damage; break;
        }

        if (nameText.text != dialogue.name && !isNameHide)
        {
            nameText.text = dialogue.name;
            jobText.text = dialogue.job;
        }

        nameText.rectTransform.anchoredPosition = new Vector2(0, nameText.rectTransform.anchoredPosition.y);
        jobText.rectTransform.anchoredPosition = new Vector2(0, jobText.rectTransform.anchoredPosition.y);

        tempText = dialogue.text;
        eventValue = dialogue.eventValue;
    }

    public IEnumerator TypingText()
    {
        if (isTyping) { isSkip = true; yield break; }
        isTyping = true;
        text.text = null;
        for (int i = 0; i < tempText.Length; i++)
        {
            if (isSkip)
            {
                text.text = tempText;
                DialogueEvent();
                isTyping = false;
                isSkip = false;
                yield break;
            }
            text.text += tempText[i];
            yield return waitTime;
        }
        isTyping = false;
        DialogueEvent();
    }

    void DialogueEvent()
    {
        //Debug.Log(curEvent);

        if (curEvent != SettingPanel) { /*Debug.Log("is Not SettingPanel");*/ OnOffPanel(eventValue); }
        if (curEvent == null) return;

        curEvent?.Invoke(eventValue);
    }

    public void SettingPanel(string value)
    {
        panelState = true;
        isPanel = true;
        OnOffPanel(value);
    }

    public void OnOffPanel(string value)
    {
        panel.rectTransform.DOSizeDelta(isPanel ? new(1589, 892) : Vector2.zero, 0.5f);
        if (isPanel)
        {
            Debug.Log("Panel");
            panel.sprite = Resources.Load<Sprite>($"Panel/{value}");
            isPanel = false;
            panelState = false;
        }
    }

    public void Skip()
    {
        isEnd = true;
        isTyping = true;
        isSkip = true;
        panel.rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
// using TMPro;
// using UnityEngine.UI;
// using System;

// public class ScenarioManager : MonoBehaviour
// {
//     public enum NamePos
//     {
//         Left,
//         Right,
//         Hide
//     }

//     public enum CamPos
//     {
//         LeftEnd,
//         Left,
//         Middle,
//         Right,
//         RightEnd
//     }

//     public enum DialogueEvent
//     {
//         Null,
//         Panel,
//         Player_Damage,
//         Enemy_Damage,
//     }

//     public static ScenarioManager instance { get; private set; }

//     [SerializeField] Image panel;

//     [Header("Dialogue")]
//     [SerializeField] private GameObject dialogueUI;
//     [SerializeField] private GameObject leftBox;
//     [SerializeField] private GameObject rightBox;
//     [SerializeField] private TextMeshProUGUI text;
//     [SerializeField] private TextMeshProUGUI leftNameText;
//     [SerializeField] private TextMeshProUGUI leftJobText;
//     [SerializeField] private TextMeshProUGUI rightNameText;
//     [SerializeField] private TextMeshProUGUI rightJobText;
//     [SerializeField] private GameObject focusUI;
//     [SerializeField] private float typingTime;

//     bool isSkip;

//     public void Skip()
//     {
//         isSkip = true;
//     }

//     private void Awake()
//     {
//         instance = this;
//         text.text = null;
//     }

//     public void StartScenario(ScenarioData scenario)
//     {
//         StartCoroutine(ScenarioRoutine(scenario));
//     }

//     private IEnumerator ScenarioRoutine(ScenarioData scenario)
//     {
//         //scenario initialize
//         dialogueUI.SetActive(true);
//         focusUI.SetActive(true);

//         for (int dialogueIndex = 0; dialogueIndex < scenario.dialogues.Length; dialogueIndex++)
//         {
//             //dialogue initialize
//             panel.gameObject.SetActive(false);

//             var dialogue = scenario.dialogues[dialogueIndex];

//             SetCamPos(dialogue);
//             SetNamePos(dialogue);

//             //text typing
//             text.text = dialogue.text;
//             for (int curTextLength = 0; curTextLength < dialogue.text.Length; curTextLength++)
//             {
//                 text.textInfo.characterCount = curTextLength;
//                 var curTypingTime = 0f;
//                 while (curTypingTime <= typingTime)
//                 {
//                     curTypingTime += Time.deltaTime;
//                     if (isSkip) break;
//                     yield return null;
//                 }
//                 if (isSkip) break;
//             }
//             isSkip = false;

//             ExecuteDialogueEvent(dialogue);
//             yield return new WaitForSeconds(0.5f);
//             yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
//         }

//         //hide
//         dialogueUI.SetActive(false);
//         focusUI.SetActive(false);
//         rightBox.SetActive(false);
//         leftBox.SetActive(false);

//         yield return null;
//     }

//     private void SetNamePos(DialogueData dialogue)
//     {
//         switch (dialogue.namePos)
//         {
//             case NamePos.Left:
//                 leftNameText.text = dialogue.name;
//                 leftJobText.text = dialogue.job;
//                 leftBox.SetActive(true);
//                 rightBox.SetActive(false);
//                 break;
//             case NamePos.Right:
//                 rightNameText.text = dialogue.name;
//                 rightJobText.text = dialogue.job;
//                 rightBox.SetActive(true);
//                 leftBox.SetActive(false);
//                 break;
//             case NamePos.Hide:
                
//                 break;
//         }
//     }

//     private void SetCamPos(DialogueData dialogue)
//     {
//         // switch (dialogue.camPos)
//         // {
//         //     case CamPos.LeftEnd: uiManager.camPlusPos = new Vector3(-3, -1); break;
//         //     case CamPos.Left: uiManager.camPlusPos = new Vector3(-1, -1); break;
//         //     case CamPos.Middle: uiManager.camPlusPos = new Vector3(0, -1); break;
//         //     case CamPos.Right: uiManager.camPlusPos = new Vector3(1, -1); break;
//         //     case CamPos.RightEnd: uiManager.camPlusPos = new Vector3(3, -1); break;
//         // }
//     }

//     // public void InputDialogue(DialogueInfo dialogue)
//     // {
//     //     switch (dialogue.curEvent)
//     //     {
//     //         // case CurEvent.Null: curEvent = null; break;
//     //         // case CurEvent.Panel: curEvent = SettingPanel; break;
//     //         // case CurEvent.Player_Damage: curEvent = controller.player.Damage; break;
//     //         // case CurEvent.Enemy_Damage: curEvent = controller.enemy.Damage; break;
//     //     }
//     //     // eventValue = dialogue.eventValue;
//     // }

//     private void ExecuteDialogueEvent(DialogueData dialogue)
//     {
//         string command = null;
//         string value = null;

//         int valueStartIndex = 0;
//         for (int i = 0; i < dialogue.eventValue.Length; i++)
//         {
//             if (dialogue.eventValue[i] == '(')
//             {
//                 command = dialogue.eventValue[0..i];
//                 i++;
//                 valueStartIndex = i;
//             }

//             if (dialogue.eventValue[i] == ')')
//                 value = dialogue.eventValue[valueStartIndex..i];
//         }

//         switch (command)
//         {
//             case "Tutorial":
//                 panel.sprite = Resources.Load<Sprite>($"Panel/{value}");
//                 panel.gameObject.SetActive(true);
//                 break;
//         }
//     }
// }
