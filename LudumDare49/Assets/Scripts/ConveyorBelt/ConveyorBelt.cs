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

    List<AnimalController.AnimalDef> itemQueue = new List<AnimalController.AnimalDef>();

    float timer = 0f;
    float nextItemCreation = 0f;

    void Start()
    {
        PopulateItemQueue();
    }

    public void Reset()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
        itemQueue.Clear();
        timer = 0f;
        nextItemCreation = 0f;
        PopulateItemQueue();
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
            CreateItem(GetNextItemInQueue(), Random.Range(2, 6));
        }
    }

    void PopulateItemQueue()
    {
        List<AnimalController.AnimalDef> defs = AnimalController.Instance.AnimalDefs;
        itemQueue.AddRange(defs);
        itemQueue.AddRange(defs);

        itemQueue.Sort(RandomSort);
    }

    AnimalController.AnimalDef GetNextItemInQueue()
    {
        if (itemQueue.Count == 0)
        {
            PopulateItemQueue();
        }

        AnimalController.AnimalDef item = itemQueue[0];
        itemQueue.RemoveAt(0);

        return item;
    }

    int RandomSort(AnimalController.AnimalDef A, AnimalController.AnimalDef B)
    {
        return Random.Range(-1, 2);
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
