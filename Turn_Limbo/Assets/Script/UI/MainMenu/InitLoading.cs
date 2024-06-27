using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InitLoading : MonoBehaviour, IInitObserver
{
    [SerializeField] SkillUpgrade skillUpgrade;
    [SerializeField] Image LoadImage;

    public int Priority => 0;

    public void Init()
    {
        if (!ReadSpreadSheet.instance.isFirstLoad)
        {
            LoadImage.gameObject.SetActive(true);
            LoadImage.color = Color.black;
            ReadSpreadSheet.instance.Load(() =>
            {
                skillUpgrade.AddSkillUpgradeBtn();
                LoadImage.DOColor(Color.clear, 1f).SetEase(Ease.Linear).OnComplete(() => LoadImage.gameObject.SetActive(false));
                ReadSpreadSheet.instance.isFirstLoad = true;
            });
        }
        else
        {
            skillUpgrade.AddSkillUpgradeBtn();
        }
    }
}
