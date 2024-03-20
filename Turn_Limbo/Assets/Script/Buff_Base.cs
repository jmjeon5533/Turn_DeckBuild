using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff_Base : MonoBehaviour
{
    public Sprite buffIcon;

    public abstract void Use(Unit target, int stack, Unit.PropertyType type);
}
