using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    private void Update()
    {
        audioSource.volume = SoundManager.soundVolume;
    }
}
