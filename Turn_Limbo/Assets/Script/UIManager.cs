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
    [SerializeField] Controller controller;
    [SerializeField] Image[] keys;
    
    [SerializeField] Image baseIcon;

    private void Update() {
        cam.transform.position = Vector3.Lerp(cam.transform.position,controller.movePos + (Vector3.forward * -10),0.2f);
        if(controller.isAttack)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,3,0.2f);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,5,0.2f);
        }
    }
    public Image AddImage(Sprite sprite,Transform parent)
    {
        var img = Instantiate(baseIcon,parent);
        img.sprite = sprite;
        return img;
    }
    public void TriggerBtn(bool isActive)
    {
        for(int i = 0; i < keys.Length; i++)
        {
            keys[i].enabled = isActive;
        }
    }
    public void NextImage(int index,Sprite sprite)
    {
        keys[index].sprite = sprite;
    }
}
