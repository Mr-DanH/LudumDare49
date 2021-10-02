using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Features:
    Each item will have 1 type of animal with a random amount of them.
*/
public class ConveyorBeltItem : MonoBehaviour
{
    [SerializeField] Text type;
    [SerializeField] Text countdown;
    [SerializeField] Text spawnNum;

    public float TimeOut { get; private set; }
    public int ItemType { get; private set; }

    public void Init(float timeOut, int itemType, int numToSpawn)
    {
        TimeOut = timeOut;
        ItemType = itemType;
        type.text = itemType.ToString();
        spawnNum.text = numToSpawn.ToString();
    }

    void Update()
    {
        float duration = TimeOut - Time.deltaTime;
        countdown.text = $"{duration.ToString("%s")}s";
    }
}
