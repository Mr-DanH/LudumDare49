using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : Singleton<Island>
{
    [SerializeField] float radius = 200f;
     public float Radius { get { return radius; } }

}