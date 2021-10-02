using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : Singleton<Island>
{
    [SerializeField] float radius = 200f;
     public float Radius { get { return radius; } }

    public Vector2 GetRandomMoveTarget(float minDegrees = 0, float maxDegrees = 360)
    {
        float angle = Random.Range(minDegrees, maxDegrees) * Mathf.Deg2Rad;
        float radius = Random.Range(Radius * 0.33f, Radius);
        return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
    }

}