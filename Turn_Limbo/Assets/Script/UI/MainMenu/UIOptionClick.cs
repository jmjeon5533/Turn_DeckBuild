using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIOptionClick : MonoBehaviour, IInitObserver
{
    [SerializeField] private Button btn;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Slider volumeSlider;
    public int Priority => 2;
    bool isOn;

    public void Init()
    {
        btn.onClick.AddListener(() => OptionOnOff());
        SoundManager.soundVolume = volumeSlider.value;
    }
    public void OptionOnOff()
    {
        isOn = !isOn;
        optionPanel.SetActive(isOn);
    }
    private void Update()
    {
        SoundManager.soundVolume = volumeSlider.value;
    }
}
