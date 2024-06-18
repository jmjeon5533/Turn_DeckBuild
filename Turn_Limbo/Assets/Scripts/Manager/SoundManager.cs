using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance{get; private set;}
    public enum SoundState
    {
        BGM,
        SFX
    }
    public AudioSource soundObj;
    private void Awake()
    {
        instance = this;
    }

    public void SetAudio(AudioClip clip, bool looping, float pitch = 1)
    {
        var sound = Instantiate(soundObj,Vector3.zero,Quaternion.identity);
        sound.clip = clip;
        sound.loop = looping;
        sound.pitch = pitch;
        sound.Play();
        if(!looping) Destroy(sound.gameObject,sound.clip.length);
    }
}
