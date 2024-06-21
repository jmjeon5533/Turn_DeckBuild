using UnityEngine;
using UnityEngine.UI;

public class RemainActUI : MonoBehaviour
{
    private Player player => Player.instance;

    [SerializeField] private Image bar;

    private void Update()
    {
        bar.fillAmount = player.Act / 10f;
    }
}