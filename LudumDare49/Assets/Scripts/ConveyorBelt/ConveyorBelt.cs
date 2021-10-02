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
    [SerializeField] float itemCreationDelay;

    List<ConveyorBeltItem> items = new List<ConveyorBeltItem>();

    float timer = 0f;
    float itemLastCreated = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        RefreshConveyorItems();
    }

    void RefreshConveyorItems()
    {
        foreach(var item in items)
        {
            item.Refresh(timer);
        }

        // has an item timed out and isn't being currently used
        RemoveDeadItems();

        // does a new item need created? 
        bool shouldCreateNextItem = items.Count < maxBeltItems && itemLastCreated < timer;
        
        if (shouldCreateNextItem)
        {
            CreateItem();
        }
    }

    void CreateItem()
    {
        ConveyorBeltItem clone = Instantiate<ConveyorBeltItem>(itemTemplate, belt);
        // todo - find somewhere that tells us how many are spawned from the item
        float timeOut = timer + itemDuration;
        clone.Init(timeOut, Random.Range(1, 5));
        items.Add(clone);
        itemLastCreated = timer + itemCreationDelay;
    }

    void RemoveDeadItems()
    {
        List<ConveyorBeltItem> timedOutItems = items.FindAll(x=>timer >= x.TimeOut || x.Used);
        foreach (var item in timedOutItems)
        {
            RemoveItem(item);
        }
    }

    void RemoveItem(ConveyorBeltItem item)
    {
        items.Remove(item);
        Destroy(item.gameObject);
    }
}
