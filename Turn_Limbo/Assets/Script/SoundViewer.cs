using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundViewer : MonoBehaviour
{
    [SerializeField] AudioClip sounds;
    [SerializeField] bool looping;
    void Start()
    {
        SoundManager.instance.SetAudio(sounds,looping);
    }
}
