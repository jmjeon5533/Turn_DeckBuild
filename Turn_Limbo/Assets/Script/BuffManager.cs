using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance{get; private set;}

    public List<Buff_Base> buffList;
    public List<Buff_Base> debuffList;

    private void Awake() {
        instance = this;
    }
}
