using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Features:
    Contains items that it will pool and generate as more are needed.
    Control the speed of the conveyor belt as in how long the items are available for.
     -- First pass add timers to the items, oldest to newest
*/
public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] Transform belt;
    [SerializeField] ConveyorBeltItem itemTemplate;
    [SerializeField] int maxBeltItems;
    [SerializeField] float itemDuration;

    List<ConveyorBeltItem> items;
    float timer;

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        RefreshConveyorItems();
    }

    void RefreshConveyorItems()
    {
        // has an item timed out and isn't being currently used
        RemoveTimedOutItems();

        // does a new item need created? 
        // first lets create with no delay

        bool shouldCreateNextItem = items.Count < maxBeltItems;
        
        if (shouldCreateNextItem)
        {
            CreateItem();
        }
    }

    void CreateItem()
    {
        ConveyorBeltItem clone = Instantiate<ConveyorBeltItem>(itemTemplate, belt);
        // todo - find somewhere that tells us how many animals we're using and how many are spawned from the item
        clone.Init(timer += itemDuration, Random.Range(0, 5), Random.Range(0, 5));
        items.Add(clone);
    }

    void RemoveItem(ConveyorBeltItem item)
    {
        items.Remove(item);
        Destroy(item.gameObject);
    }

    void RemoveTimedOutItems()
    {
        List<ConveyorBeltItem> timedOutItems = items.FindAll(x=> timer >= x.TimeOut);
        foreach (var item in timedOutItems)
        {
            RemoveItem(item);
        }
    }
}
