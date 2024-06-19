using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlaterPrefsButtonTest : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
