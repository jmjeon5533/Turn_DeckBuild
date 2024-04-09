using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogView : MonoBehaviour
{
    private RectTransform UIPanel;
    private bool isPanelMove;
    [SerializeField] RectTransform[] Panels;
    [SerializeField] GameObject[] BaseObjs;
    [Space(10)]
    [SerializeField] RectTransform versers;
    [SerializeField] Image baseVersers;
    bool a, b, c, d, e;

    private void Start()
    {
        UIPanel = GetComponent<RectTransform>();
    }
    public void SetPanel()
    {
        if (isPanelMove) return;
        isPanelMove = true;
        StartCoroutine(PanelSet());
    }
    private IEnumerator PanelSet()
    {
        yield return UIPanel.DOLocalMove(Vector2.zero, 1).SetEase(Ease.OutQuad).OnComplete(() => isPanelMove = false).WaitForCompletion();
    }
    public void AddLogs()
    {
        
    }
}
