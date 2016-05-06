using UnityEngine;
using System.Collections;

public static class VMath  {

    public static float DistanceBetween(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    public static bool DistWithinThresh(Vector3 a, Vector3 b, float thresh)
    {
        return DistanceBetween(a, b) < thresh * thresh;
    }


}
