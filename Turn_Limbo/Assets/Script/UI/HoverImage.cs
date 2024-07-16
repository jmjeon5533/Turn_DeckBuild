using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private int i;
    public void OnPointerEnter(PointerEventData eventData)
    {
        var ui = UIManager.instance;
        if (controller.player.skillInfo.holdSkills.TryGetValue(controller.inputs[i][0].index - 1, out var holdSkill))
            ui.SetExplain(true, controller.inputs[i][0], ui.keys[i].rectTransform.anchoredPosition, holdSkill.level);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        var ui = UIManager.instance;
        ui.SetExplain(false);
    }
}
