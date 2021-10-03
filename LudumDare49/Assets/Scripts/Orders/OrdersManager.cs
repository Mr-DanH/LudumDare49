using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersManager : MonoBehaviour
{
    class Port
    {
        public Transform Point;
        public Order CurrentOrder;

        public Port(Transform point)
        {
            Point = point;
            CurrentOrder = null;
        }

        public bool HasOrder()
        {
            return CurrentOrder != null;
        }
    }

    [SerializeField] Order orderTemplate;
    [SerializeField] List<Transform> portPoints;
    [SerializeField] int minOrderFulfillmentAmount;
    [SerializeField] int maxOrderFulfillmentAmount;

    List<Port> ports = new List<Port>();

    void Start()
    {
        foreach (Transform point in portPoints)
        {
            ports.Add(new Port(point));
        }
    }

    void Update()
    {
        EnsureEnoughOrders();
        RefreshOrders();
    }

    void RefreshOrders()
    {
       Dictionary<AnimalController.AnimalDef, int> intel = AnimalController.Instance.GatherAnimalIntel();
       foreach (var port in ports)
       {
           if (port.HasOrder())
           {
                Order order = port.CurrentOrder;
                intel.TryGetValue(order.AnimalDef, out int numOnIsland);
                order.Refresh(numOnIsland);
                if (order.Collected)
                {
                    // todo - points!
                    port.CurrentOrder = null;
                    Destroy(order.gameObject);
                }
           }
       }
    }

    void EnsureEnoughOrders()
    {
        Port emptyPort = ports.Find(x=>!x.HasOrder());
        if (emptyPort != null)
        {
           Order generatedOrder = GenerateRandomOrder(emptyPort);
           generatedOrder.gameObject.SetActive(true);
           // Todo - maybe check whether we want to use this order?
           emptyPort.CurrentOrder = generatedOrder;
        }
    }

    Order GenerateRandomOrder(Port port)
    {
        AnimalController.AnimalDef animalDef = AnimalController.Instance.GetRandomAnimalDef();
        Order clone = Instantiate<Order>(orderTemplate, port.Point);
        int fulfillmentAmount = Random.Range(minOrderFulfillmentAmount, maxOrderFulfillmentAmount);
        clone.Init(animalDef, fulfillmentAmount);
        return clone;
    }
}
