using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class INT3
{
    public static int3 Difference(int3 a, int3 b)
    {
        int3 output = new int3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        output.x -= Mathf.Abs(b.x);
        output.y -= Mathf.Abs(b.y);
        output.z -= Mathf.Abs(b.z);

        return output;
    }
    public static int3 Clamp(int3 a, int min, int max)
    {
        int3 outPut = new int3(Mathf.Clamp(a.x, min, max), Mathf.Clamp(a.y, min, max), Mathf.Clamp(a.z, min, max));

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
    public static bool IsZero(int3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
}
