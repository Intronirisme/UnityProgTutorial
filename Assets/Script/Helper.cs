using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Transform FindInChildren(this Transform self, string name)
    {
        int count = self.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = self.GetChild(i);
            if (child.name == name) return child;
            Transform subChild = child.FindInChildren(name);
            if (subChild != null) return subChild;
        }
        return null;
    }

    public static GameObject FindInChildren(this GameObject self, string name)
    {
        Transform transform = self.transform;
        Transform child = transform.FindInChildren(name);
        return child != null ? child.gameObject : null;
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
}