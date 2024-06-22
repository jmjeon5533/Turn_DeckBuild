using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngameDeckQueueDisplayer : MonoBehaviour
{
    [System.Serializable]
    public struct LerpData
    {
        public Vector2 position;
        public Vector2 size;
        public Color color;
    }

    private Player player => Player.instance;
    private DataManager dataManager => DataManager.instance;

    private readonly float ANIMATION_SPEED = 10;

    [SerializeField] private Image[] images;
    [SerializeField] private int index;
    [SerializeField] private LerpData[] lerpTargetData;
    [SerializeField] private LerpData[] lerpChangedData;

    private void Start()
    {
        this.Invoke(() =>
        {
            player
                .ObserveEveryValueChanged(p => p.DeckQueues[index][0])
                .Subscribe(_ =>
                {
                    for(int i = 0; i < 3; i++)
                    {
                        images[i].sprite = i == 0 ? images[1].sprite : dataManager.loadData.ActionDatas[player.DeckQueues[index][i - 1]].icon;
                        images[i].rectTransform.anchoredPosition = lerpChangedData[i].position;
                        images[i].rectTransform.sizeDelta = lerpChangedData[i].size;
                        images[i].color = lerpChangedData[i].color;
                    }
                });
        }, 0);
    }

    private void Update()
    {
        for(int i = 0; i < 3; i++)
        {
            images[i].rectTransform.anchoredPosition =
                Vector2.Lerp(images[i].rectTransform.anchoredPosition, lerpTargetData[i].position, Time.deltaTime * ANIMATION_SPEED);
            images[i].rectTransform.sizeDelta =
                Vector2.Lerp(images[i].rectTransform.sizeDelta, lerpTargetData[i].size, Time.deltaTime * ANIMATION_SPEED);
            images[i].color =
                Color.Lerp(images[i].color, lerpTargetData[i].color, Time.deltaTime * ANIMATION_SPEED);
        }
    }
}