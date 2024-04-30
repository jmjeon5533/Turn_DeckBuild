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
    [SerializeField] List<Sprite> panelImage = new();

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
    int eventValue;
    bool isSkip;
    bool isPanel;
    bool isNameHide;
    Action<int> curEvent;

    //caching
    Vector3 nameBarLeft = new(-610, 100);
    Vector3 nameBarRight = new(610, 100);

    public void OnOffDialogue(bool isOn)
    {
        if (isOn)
        {
            //cam.DOOrthoSize(3.5f, 0.5f).SetEase(Ease.OutCubic);
            nameText.text = null;
            text.text = null;
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
        barSize.rectTransform.DOSizeDelta(isOn ? new(0, 450) : Vector2.zero, 0.5f);
    }

    public void InputDialogue(Dialogue dialogue)
    {
        switch (dialogue.namePos)
        {
            case NamePos.Left:
                nameLeftBar.rectTransform.DOLocalMoveX(-610, 0.5f);
                nameRightBar.rectTransform.DOLocalMoveX(1310, 0.5f);
                nameText.transform.parent = nameLeftBar.transform;
                jobText.transform.parent = nameLeftBar.transform;
                isNameHide = false; break;

            case NamePos.Right:
                nameRightBar.rectTransform.DOLocalMoveX(610, 0.5f);
                nameLeftBar.rectTransform.DOLocalMoveX(-1310, 0.5f);
                nameText.transform.parent = nameRightBar.transform;
                jobText.transform.parent = nameRightBar.transform;
                isNameHide = false; break;

            case NamePos.Hide:
                if (isNameHide) break;
                else
                {
                    nameRightBar.rectTransform.DOLocalMoveX(1310, 0.5f);
                    nameLeftBar.rectTransform.DOLocalMoveX(-1310, 0.5f);
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
        if (curEvent == null) return;

        curEvent?.Invoke(eventValue);
    }

    public void SettingPanel(int isOn)
    {
        panelState = true;
        isPanel = true;
    }

    public void OnOffPanel()
    {
        panel.rectTransform.DOSizeDelta(isPanel ? new(1500, 500) : Vector2.zero, 0.5f);
        if (isPanel)
        {
            panel.sprite = panelImage[0];
            panelImage.Remove(panelImage[0]);
            isPanel = !isPanel;
        }
        else
        {
            panelState = false;
        }
    }
}
