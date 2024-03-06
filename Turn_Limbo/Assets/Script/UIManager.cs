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
    public RectTransform RequestPos;
    public Camera cam;
    [SerializeField] Transform player;
    [SerializeField] Image[] keys;
    
    [SerializeField] Image baseIcon;

    private void Update() {
        RequestPos.anchoredPosition 
        = cam.WorldToScreenPoint(player.position + new Vector3(3,2.5f + (5 - cam.orthographicSize) * 0.5f));
        RequestPos.localScale = Vector3.one * (1 + (5 - cam.orthographicSize) * 0.3f);
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
