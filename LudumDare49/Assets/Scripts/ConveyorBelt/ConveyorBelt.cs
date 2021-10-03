using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Singleton<ConveyorBelt>
{
    [SerializeField] Transform belt;
    [SerializeField] ConveyorBeltItem itemTemplate;
    [SerializeField] int maxBeltItems;
    [SerializeField] float minItemCreationDelay = 1f;
    [SerializeField] float maxItemCreationDelay = 5f;
    [SerializeField] float itemDuration;

    List<ConveyorBeltItem> items = new List<ConveyorBeltItem>();

    float timer = 0f;
    float nextItemCreation = 0f;

    public void CreateSpecificItem(AnimalController.AnimalDef animalDef, int numToSpawn)
    {
        CreateItem(animalDef, numToSpawn);
    }

    public void Reset()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
        timer = 0f;
        nextItemCreation = 0f;
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
        nextItemCreation = timer + Random.Range(minItemCreationDelay, maxItemCreationDelay);
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
