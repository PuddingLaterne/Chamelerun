﻿using UnityEngine;
using System.Collections;
using System.IO;

public static  class ExtensionMethods
{
    public static bool Approximately(this float value, float targetValue, float epsilon = 0.001f)
    {
        return Mathf.Abs(value - targetValue) < epsilon;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float GetAngle(this Vector2 v)
    {
        Vector2 a = v.normalized;
        Vector2 b = Vector2.up;
        return Mathf.Atan2(b.y -a.y, b.x - a.x) * Mathf.Rad2Deg * 2f;
    }

    public static float Ratio(this float a, float b)
    {
        return a / (a + b);
    }

    public static float SignedSquared(this float value)
    {
        return Mathf.Sign(value) * Mathf.Pow(value, 2);
    }

    public static int ToBitmask(this int x)
    {
        return (int)Mathf.Pow(2, x);
    }

    public static bool IsPartOfBitmask(this int x, int mask)
    {
        return (x & mask) != 0;
    }
}