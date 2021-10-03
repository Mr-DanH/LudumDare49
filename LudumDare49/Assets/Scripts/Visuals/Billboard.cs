using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
    void Start()
    {
        if(Island.Instance != null)
            Island.Instance.RegisterBillboard(this);
    }

    void OnDestroy()
    {
        if(Island.Instance != null)
            Island.Instance.UnregisterBillboard(this);
    }

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
