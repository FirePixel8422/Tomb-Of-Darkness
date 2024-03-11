using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class INT_Logic
{
    public static int3 Difference(int3 a, int3 b)
    {
        int3 output = new int3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        output.x -= Mathf.Abs(b.x);
        output.y -= Mathf.Abs(b.y);
        output.z -= Mathf.Abs(b.z);

        return output;
    }
    public static int3 Subtract(int3 a, int3 b)
    {
        int3 output = a;
        output.x -= b.x;
        output.y -= b.y;
        output.z -= b.z;

        return output;
    }
    public static int3 Abs(int3 a)
    {
        a.x = Mathf.Abs(a.x);
        a.y = Mathf.Abs(a.y);
        a.z = Mathf.Abs(a.z);

        return a;
    }
    public static int3 Clamp(int3 a, int min, int max)
    {
        int3 outPut = new int3(Mathf.Clamp(a.x, min, max), Mathf.Clamp(a.y, min, max), Mathf.Clamp(a.z, min, max));

        return outPut;
    }
    public static int2 Clamp(int2 a, int min, int max)
    {
        int2 outPut = new int2(Mathf.Clamp(a.x, min, max), Mathf.Clamp(a.y, min, max));

        return outPut;
    }
    public static int Distance(int3 a, int3 b)
    {
        int distance = 0;
        distance += Mathf.Abs(a.x + b.x);
        distance += Mathf.Abs(a.y + b.y);
        distance += Mathf.Abs(a.z + b.z);

        return distance;
    }
    public static Vector3 Vector3Distance(int3 a, int3 b)
    {
        Vector3 vec = new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));

        vec.x -= Mathf.Abs(b.x);
        vec.y -= Mathf.Abs(b.y);
        vec.z -= Mathf.Abs(b.z);

        return vec;
    }
    public static bool IsZero(int3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
}
