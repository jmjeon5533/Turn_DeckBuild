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
    public Camera cam;
    [SerializeField] Transform player;
    [SerializeField] Image baseIcon;
    [SerializeField] Image[] keys;

    private void Update() {
        RequestPos.localPosition 
        = Camera.main.WorldToScreenPoint(player.localPosition + new Vector3(-7,0 + (5 - cam.orthographicSize)));
    }

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
