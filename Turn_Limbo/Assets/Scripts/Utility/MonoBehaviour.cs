using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void Invoke(this MonoBehaviour mb, Action action, float t)
    {
        mb.StartCoroutine(Routine(action, t));
        IEnumerator Routine(Action action, float t)
        {
            yield return new WaitForSeconds(t);
            action?.Invoke();
        }
    }
}