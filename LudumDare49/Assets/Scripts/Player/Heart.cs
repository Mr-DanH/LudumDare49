using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] Image fill;

    public void UpdateFill(float fillAmount)
    {
        fill.fillAmount = fillAmount;
    }
}
