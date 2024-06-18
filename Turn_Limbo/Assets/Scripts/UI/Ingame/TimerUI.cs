using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image bar;

    private void Update()
    {
        // bar.fillAmount = controller.gameCurTimeCount / 10f;
    }
}
