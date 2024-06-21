using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    private GameManager actionManager => GameManager.instance;

    [SerializeField] private Image bar;

    private void Update()
    {
        bar.fillAmount = actionManager.AttackRemainTime / 10f;
    }
}
