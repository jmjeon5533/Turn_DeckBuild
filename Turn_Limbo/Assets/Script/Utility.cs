using System;
using UnityEngine;

public static class Utility
{
    public static T EnumParse<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value);
    }
    public static Color MoveToward(this Color value, Color target, float maxDelta)
    {
        float r, g, b, a;
        r = Mathf.MoveTowards(value.r, target.r, maxDelta);
        g = Mathf.MoveTowards(value.g, target.g, maxDelta);
        b = Mathf.MoveTowards(value.b, target.b, maxDelta);
        a = Mathf.MoveTowards(value.a, target.a, maxDelta);
        Color color = new Color(r, g, b, a);
        //Debug.Log($"[{target}]{value} => {color}");
        return color;
    }
}