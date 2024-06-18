using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatMove : MonoBehaviour
{
    private enum MoveState
    {
        Object,
        UI
    }
    [SerializeField] private MoveState moveState;
    [SerializeField] private float moveRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 dir;
    Vector3 anchorPos;
    RectTransform rect;
    void Start()
    {
        if (moveState == MoveState.UI)
        {
            rect = GetComponent<RectTransform>();
            anchorPos = rect.anchoredPosition;
        }
        else anchorPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveState == MoveState.Object) transform.position = anchorPos + (dir * Mathf.Sin(Time.time * moveSpeed) * moveRange);
        else rect.anchoredPosition = anchorPos + (dir * Mathf.Sin(Time.time * moveSpeed) * moveRange);
    }
}
