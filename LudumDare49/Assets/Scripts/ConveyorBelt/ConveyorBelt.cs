using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Features:
    Contains items that it will pool and generate as more are needed.
    Control the speed of the conveyor belt as in how long the items are available for.
     -- First pass add timers to the items, oldest to newest
*/
public class ConveyorBelt : Singleton<ConveyorBelt>
{
    [SerializeField] Transform belt;
    [SerializeField] ConveyorBeltItem itemTemplate;
    [SerializeField] int maxBeltItems;
    [SerializeField] float maxItemCreationDelay;
    [SerializeField] float itemDuration;

    List<ConveyorBeltItem> items = new List<ConveyorBeltItem>();

    float timer = 0f;
    float nextItemCreation = 0f;

    public void CreateSpecificItem(AnimalController.AnimalDef animalDef, int numToSpawn)
    {
        CreateItem(animalDef, numToSpawn);
    }

    void Update()
    {
        timer += Time.deltaTime;
        RefreshConveyorItems();
    }

    void RefreshConveyorItems()
    {
        MoveBelt();

        RemoveDeadItems();

        bool shouldCreateNextItem = items.Count < maxBeltItems && nextItemCreation < timer;
        
        if (shouldCreateNextItem)
        {
            // todo - find somewhere that tells us how many are spawned from the item
            CreateItem(AnimalController.Instance.GetRandomAnimalDef(), Random.Range(1, 5));
        }
    }

    void MoveBelt()
    {
        float deltaTime = Time.deltaTime;
        foreach (ConveyorBeltItem item in items)
        {
            item.MoveItem(deltaTime, itemDuration);
        }
    }

    void CreateItem(AnimalController.AnimalDef animalDef, int numToSpawn)
    {
        ConveyorBeltItem clone = Instantiate<ConveyorBeltItem>(itemTemplate, belt);
        clone.Init(numToSpawn, animalDef);
        items.Add(clone);
        nextItemCreation = timer + Random.Range(0.75f, maxItemCreationDelay);
    }

    void RemoveDeadItems()
    {
        List<ConveyorBeltItem> timedOutItems = items.FindAll(x => x.Removable);
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
