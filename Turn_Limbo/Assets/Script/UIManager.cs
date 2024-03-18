using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance {get; private set;}
    private void Awake()
    {
        instance = this;
    }
    public float camRotZ = 0;
    public bool isCamRotate;
    public Camera cam;
    public Camera bgCam;
    
    public Image inputPanel;
    public Image[] keys;
    [SerializeField] Image[] nextKeys;
    [SerializeField] Controller controller;
    [SerializeField] TMP_Text coinText;
    
    [SerializeField] Transform dmgTextParent;
    [SerializeField] Image baseIcon;
    [SerializeField] Image timer;
    public TMP_Text damageText;

    private void Update()
    {
        if(!controller.isGame) return;
        if(isCamRotate) 
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(Vector3.forward * camRotZ), 0.05f);
        Vector3 camPos; 
        if(controller.isAttack)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,3.5f,0.1f);
            camPos = new Vector3(0, 0,-10);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,6 - Mathf.InverseLerp(10,0,controller.gameCurTimeCount),0.1f);
            timer.fillAmount = controller.gameCurTimeCount / 10;
            camPos = new Vector3(0, -1.5f,-10);
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position,controller.movePos + camPos,0.1f);
        bgCam.orthographicSize = cam.orthographicSize;
    }
    public void FatalDamage()
    {
        controller.glitch.intensity.value = 1;
        controller.color.saturation.value = -80;
        controller.color.postExposure.value = 1;
    }
    public Image AddImage(Sprite sprite,Transform parent)
    {
        var img = Instantiate(baseIcon,parent);
        img.sprite = sprite;
        return img;
    }
    public void ChangeCoinSkillImg()
    {
        bool isActiveBtn = controller.useAbleCoin > 0;
        for(int i = 0; i < keys.Length; i++)
        {
            keys[i].color = isActiveBtn ? Color.white : Color.grey;
        }
        coinText.text = $"Coin : {controller.useAbleCoin}";
    }
    public void ActiveBtn(bool isActive)
    {
        for(int i = 0; i < keys.Length; i++)
        {
            keys[i].enabled = isActive;
            nextKeys[i].enabled = isActive;
        }
        coinText.enabled = isActive;
        timer.enabled = isActive;
    }
    public void NextImage(int index,Sprite sprite, Sprite nextSprite)
    {
        keys[index].sprite = sprite;
        nextKeys[index].sprite = nextSprite;
    }
    public void DamageText(int damage,Vector3 pos)
    {
        var text = Instantiate(damageText,dmgTextParent);
        text.text = damage.ToString();
        
        text.rectTransform.anchoredPosition = cam.WorldToScreenPoint(pos + (Vector3)Random.insideUnitCircle * 1.5f);

        text.transform.DOScale(0,0.8f);
        text.DOColor(Color.clear,0.8f).OnComplete(() => Destroy(text.gameObject));
    }
}
