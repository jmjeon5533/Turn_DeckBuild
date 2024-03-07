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
    public Camera cam;
    [SerializeField] Image[] keys;
    
    [SerializeField] Image baseIcon;


    public Image AddImage(Sprite sprite,Transform parent)
    {
        var img = Instantiate(baseIcon,parent);
        img.sprite = sprite;
        return img;
    }
    public void NextImage(int index,Sprite sprite)
    {
        keys[index].sprite = sprite;
    }
}
