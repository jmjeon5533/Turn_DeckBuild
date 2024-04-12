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
    public int eventValue;
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
        dialogueText.text = null;
        dialogueName.text = null;
        dialogueJob.text = null;
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

    [SerializeField] Controller controller;
    [SerializeField] Image panel;
    [SerializeField] List<Sprite> panelImage = new();

    [Header("Dialogue")]
    [SerializeField] Image dialogueBar;
    [SerializeField] Image dialogueNameBar;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text dialogueName;
    [SerializeField] TMP_Text dialogueJob;
    [SerializeField] GameObject focusUI;
    private string tempText;
    [SerializeField] float typingTime;
    WaitForSeconds waitTime;
    [HideInInspector] public bool isTyping;
    [HideInInspector] public bool isPanel;
    int eventValue;
    bool isSkip;
    Action<int> curEvent;

    public void OnOffDialogue(bool isOn)
    {
        if (isOn)
        {
            //cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
            dialogueName.text = null;
            dialogueText.text = null;
            UIManager.instance.timerBG.gameObject.SetActive(!isOn);
            focusUI.SetActive(isOn);
        }
        else
        {
            UIManager.instance.camPlusPos = Vector3.zero;
            UIManager.instance.timerBG.gameObject.SetActive(isOn);
            focusUI.SetActive(!isOn);
        }
        UIManager.instance.inputPanel.rectTransform.DOSizeDelta(isOn ? Vector2.zero : new(0, 250), 0.5f);
        dialogueBar.rectTransform.DOSizeDelta(isOn ? new(0, 250) : Vector2.zero, 0.5f);
        dialogueNameBar.rectTransform.DOSizeDelta(isOn ? new(700, 150) : Vector2.zero, 0.5f);
    }

    public void InputDialogue(Dialogue dialogue)
    {
        if (dialogueName.text != dialogue.name)
        {
            dialogueName.text = dialogue.name;
            dialogueJob.text = dialogue.job;
        }

        tempText = dialogue.text;

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
            case CurEvent.Panel: curEvent = OnOffPanel; break;
            //case CurEvent.Player_Damage: curEvent = controller.player.Damage; break;
            //case CurEvent.Enemy_Damage: curEvent = controller.enemy.Damage; break;
        }

        eventValue = dialogue.eventValue;
        Debug.Log(eventValue);
    }

    public IEnumerator TypingText()
    {
        if (isTyping) { isSkip = true; yield break; }
        isTyping = true;
        dialogueText.text = null;
        for (int i = 0; i < tempText.Length; i++)
        {
            if (isSkip)
            {
                dialogueText.text = tempText;
                DialogueEvent();
                isTyping = false;
                isSkip = false;
                yield break;
            }
            dialogueText.text += tempText[i];
            yield return waitTime;
        }
        isTyping = false;
        DialogueEvent();
    }

    void DialogueEvent()
    {
        if (curEvent == null) return;

        curEvent?.Invoke(eventValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isOn">-1 is On 0 is Off</param>
    public void OnOffPanel(int isOn)
    {
        bool isOnOff = isOn != 0;
        isPanel = isOnOff;

        // if(isOnOff) panel.sprite = panelImage[0];
        // else panelImage.Remove(panelImage[0]);

        panel.rectTransform.DOSizeDelta(isOnOff ? new(1500, 500) : Vector2.zero, 0.5f);
    }
}
