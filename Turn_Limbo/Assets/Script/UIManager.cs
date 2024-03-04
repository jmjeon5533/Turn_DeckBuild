using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    private void Awake()
    {
        instance = this;
    }
    public Transform RequestPos;
    [SerializeField] Image baseIcon;
    [SerializeField] Image[] keys;

    public Image AddImage(Sprite sprite)
    {
        var img = Instantiate(baseIcon,RequestPos);
        img.sprite = sprite;
        return img;
    }
    public void NextImage(int index,Sprite sprite)
    {
        keys[index].sprite = sprite;
    }
}
