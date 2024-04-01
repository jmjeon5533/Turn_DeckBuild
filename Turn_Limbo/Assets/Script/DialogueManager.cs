using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
        dialogueText.text = null;
        dialogueName.text = null;
    }
    public enum CamPos
    {
        LeftEnd,
        Left,
        Middle,
        Right,
        RightEnd
    }

    [Header("Dialogue")]
    [SerializeField] Image dialogueBar;
    [SerializeField] Image dialogueNameBar;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text dialogueName;
    [SerializeField] GameObject focusUI;
    //[SerializeField] TMP_Text dialogueJob;
    private string tempText;
    [SerializeField] float typingTime;
    [HideInInspector] public bool isTyping;
    bool isSkip;

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
        dialogueNameBar.rectTransform.DOSizeDelta(isOn ? new(700, 100) : Vector2.zero, 0.5f);
    }

    public void InputDialogue(Dialogue dialogue)
    {
        if (dialogueName.text != new string(dialogue.name + " / " + dialogue.job))
            dialogueName.text = new string(dialogue.name + " / " + dialogue.job);

        tempText = dialogue.text;

        switch (dialogue.pos)
        {
            case CamPos.LeftEnd:
                UIManager.instance.camPlusPos = new Vector3(-3, -1);
                break;
            case CamPos.Left:
                UIManager.instance.camPlusPos = new Vector3(-1, -1);
                break;
            case CamPos.Middle:
                UIManager.instance.camPlusPos = new Vector3(0, -1);
                break;
            case CamPos.Right:
                UIManager.instance.camPlusPos = new Vector3(1, -1);
                break;
            case CamPos.RightEnd:
                UIManager.instance.camPlusPos = new Vector3(3, -1);
                break;
        }
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
                isTyping = false;
                isSkip = false;
                yield break;
            }
            dialogueText.text += tempText[i];
            yield return new WaitForSeconds(typingTime); //Can be cached
        }
        isTyping = false;
    }
}
