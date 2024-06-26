using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitActivator : MonoBehaviour
{
    private void Start()
    {
        StartInit();
    }
    protected virtual void StartInit(){}
}
