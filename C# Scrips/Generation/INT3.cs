using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class INT3
{
    public static int3 Subtract(int3 a, int3 b)
    {
        int3 output = a;
        output.x -= b.x;
        output.y -= b.y;
        output.z -= b.z;

        return output;
    }
    public static bool IsZero(int3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
}
