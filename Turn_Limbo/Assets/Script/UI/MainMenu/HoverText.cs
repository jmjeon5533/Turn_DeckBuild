using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text enableText;
    public void OnPointerEnter(PointerEventData eventData)
    {
        enableText.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enableText.enabled = false;
    }

    void Start()
    {
        enableText.enabled = false;
    }
}
