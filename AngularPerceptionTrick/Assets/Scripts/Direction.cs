using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Direction
{

    public Vector3 vec;
    public float mag;

    public Direction(Vector3 vector, float magnitude)
    {
        vec = vector;
        mag = magnitude;
    }
}
