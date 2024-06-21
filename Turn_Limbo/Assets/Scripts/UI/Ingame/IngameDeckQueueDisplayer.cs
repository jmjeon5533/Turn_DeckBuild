using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IngameDeckQueueDisplayer : MonoBehaviour
{
    private Player player => Player.instance;
    private DataManager dataManager => DataManager.instance;

    private readonly float ANIMATION_SPEED = 10;

    [SerializeField] private Image[] images;
    [SerializeField] private Image use;
    [SerializeField] private int index;

    private void Start()
    {
        this.Invoke(() =>
        {
            player
                .ObserveEveryValueChanged(p => p.DeckQueues[index][0])
                .Subscribe(_ =>
                {
                    use.sprite = images[0].sprite;
                    use.rectTransform.anchoredPosition = new Vector2(0, 0);
                    use.color = Color.white;

                    images[0].sprite = dataManager.loadData.ActionInfos[player.DeckQueues[index][0]].icon;
                    images[0].rectTransform.anchoredPosition = new Vector2(0, 20);
                    images[0].rectTransform.sizeDelta = Vector2.one * 140;
                    images[0].color = new Color(1, 1, 1, 0.5f);

                    images[1].sprite = dataManager.loadData.ActionInfos[player.DeckQueues[index][1]].icon;
                    images[1].rectTransform.anchoredPosition = new Vector2(0, 40);
                    images[1].rectTransform.sizeDelta = Vector2.one * 170;
                    images[1].color = new Color(1, 1, 1, 0);

                });
        }, 0);
    }

    private void Update()
    {
        use.rectTransform.anchoredPosition =
            Vector2.Lerp(use.rectTransform.anchoredPosition, new Vector2(0, -60), Time.deltaTime * ANIMATION_SPEED);
        use.color =
            Color.Lerp(use.color, new Color(1, 1, 1, 0), Time.deltaTime * ANIMATION_SPEED);

        images[0].rectTransform.anchoredPosition =
            Vector2.Lerp(images[0].rectTransform.anchoredPosition, new Vector2(0, 0), Time.deltaTime * ANIMATION_SPEED);
        images[0].rectTransform.sizeDelta =
            Vector2.Lerp(images[0].rectTransform.sizeDelta, Vector2.one * 120, Time.deltaTime * ANIMATION_SPEED);
        images[0].color =
            Color.Lerp(images[0].color, Color.white, Time.deltaTime * ANIMATION_SPEED);

        images[1].rectTransform.anchoredPosition =
            Vector2.Lerp(images[1].rectTransform.anchoredPosition, new Vector2(0, 20), Time.deltaTime * ANIMATION_SPEED);
        images[1].rectTransform.sizeDelta =
            Vector2.Lerp(images[1].rectTransform.sizeDelta, Vector2.one * 140, Time.deltaTime * ANIMATION_SPEED);
        images[1].color =
            Color.Lerp(images[1].color, new Color(1, 1, 1, 0.5f), Time.deltaTime * ANIMATION_SPEED);
    }
}