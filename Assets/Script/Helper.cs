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

    public static float ClampNegPos(this float value, float min, float max)
    {
        float result;
        // get relative range +/-
        float relRange = (max - min) / 2f;
        // calculate offset
        float offset = max - relRange;
        result = ((value + 540) % 360) - 180 - offset;
        if (Mathf.Abs(result) > relRange)
        {
            result = relRange * Mathf.Sign(result) + offset;
        }
        return result;
    }

    public static float AngleBetweenVector(this Vector3 A, Vector3 B)
    {
        float result = Mathf.Acos(Vector3.Dot(A.normalized, B.normalized));
        return result * Mathf.Rad2Deg;
    }

    public static Quaternion RotationTo(this Quaternion from, Quaternion to)
    {
        return Quaternion.Inverse(from) * to;
    }

    public static Quaternion ApplyLocalRotation(this Quaternion from, Quaternion toApply)
    {
        return toApply * from;
    }

    public static Quaternion ApplyGlobalRotation(this Quaternion from, Quaternion toApply)
    {
        return from * toApply;
    }


}

//custom struct
public struct CameraControls
{
    public CameraControls(float Ho, float Az, float Dist)
    {
        Horizon = Ho;
        Azimuth = Az;
        Distance = Dist;
    }

    public float Horizon { get; set; }
    public float Azimuth { get; set; }
    public float Distance { get; set; }

    public override string ToString() => $"(Yaw: {Horizon} | Pitch: {Azimuth} | Distance: {Distance})";
}