using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAllCache : MonoBehaviour
{
    public void Clear()
    {
        PlayerPrefs.DeleteKey("SaveData");
        Application.Quit();
    }
}
