using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersManager : Singleton<OrdersManager>
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
    [SerializeField] int pointsPerOrder = 1;

    public int m_numOrders = 3;

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

    public void Reset()
    {
        foreach (var port in ports)
        {
           if (port.HasOrder())
           {
               Destroy(port.CurrentOrder.gameObject);
               port.CurrentOrder = null;
           }
        }

        orderQueue.Clear();
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
                    Game.Instance.IncreaseScore(pointsPerOrder);
                    port.CurrentOrder = null;
                    Destroy(order.gameObject);
                }
           }
       }
    }

    void EnsureEnoughOrders()
    {
        if(ports.FindAll(a => a.HasOrder()).Count < m_numOrders)
        {            
            var emptyPorts = ports.FindAll(a => !a.HasOrder());
            if(emptyPorts.Count == 0)
                return;

            int index = Random.Range(0, emptyPorts.Count);

            Port emptyPort = emptyPorts[index];

            Order generatedOrder = GenerateOrder(emptyPort);
            generatedOrder.gameObject.SetActive(true);
            emptyPort.CurrentOrder = generatedOrder;
        }
    }

    Order GenerateOrder(Port port)
    {
        if (orderQueue.Count == 0)
        {
            PopulateOrderQueue();
        }

        AnimalController.AnimalDef nextOrder = orderQueue[0];
        orderQueue.RemoveAt(0);
        Order clone = Instantiate<Order>(orderTemplate, Island.Instance.GetIslandObjectContainer());
        int fulfillmentAmount = Random.Range(minOrderFulfillmentAmount, maxOrderFulfillmentAmount);
        clone.Init(nextOrder, fulfillmentAmount, port.Point);
        return clone;
    }

    List<AnimalController.AnimalDef> orderQueue = new List<AnimalController.AnimalDef>();

    void PopulateOrderQueue()
    {
        orderQueue.AddRange(Util.PopulateAnimalQueue());
    }
}
