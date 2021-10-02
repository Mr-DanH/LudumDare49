using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float Scale { get; set; }

    void OnEnable()
    {
        transform.GetChild(0).localScale = new Vector3(Random.value > 0.5f ? 1 : -1, 1, 1);
    }

    void Update()
    {
        if(Scale < 1)
        {
            Scale = Mathf.MoveTowards(Scale, 1, Time.deltaTime * 0.25f);
            transform.localScale = Vector3.one * Scale;
        }
    }
}
