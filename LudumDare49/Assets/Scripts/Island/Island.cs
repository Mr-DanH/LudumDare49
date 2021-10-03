using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : Singleton<Island>
{
    [SerializeField] GameObject islandObjectsContainer;
    [SerializeField] float radius = 200f;
     public float Radius { get { return radius; } }
     public float InnerRadius { get { return radius * 0.33f; } }
    [SerializeField] CrateDrop crateDropTemplate;

    bool GetCircleIntersection(float radius, Vector2 from, Vector2 dir, out Vector2 point)
    {
        //float innerRadius = radius * 0.33f;

        Vector2 toMidPoint = -from;
        Vector2 toIntersectionMidpoint = Vector2.Dot(toMidPoint, dir) * dir;
        Vector2 midpoint = from + toIntersectionMidpoint;

        if(midpoint.magnitude > radius)
        {
            point = Vector2.zero;
            return false;
        }

        float distToIntersection = Mathf.Sqrt((radius * radius) - midpoint.sqrMagnitude);

        Vector2 intersectionA = midpoint - (distToIntersection * dir);
        Vector2 intersectionB = midpoint + (distToIntersection * dir);

        point = (intersectionA - from).sqrMagnitude < (intersectionB - from).sqrMagnitude ? intersectionA : intersectionB;
        return true;
    }

    public Vector2 GetRandomMoveTarget(Vector2 from, float minDegrees = 0, float maxDegrees = 360)
    {
        //If in inner circle head away from centre
        if(from.magnitude <= InnerRadius)
        {
            float angleToCentre = Mathf.Atan2(from.y, from.x) * Mathf.Rad2Deg;
            minDegrees = angleToCentre - 45;
            maxDegrees = angleToCentre + 45;
        }

        //If on outer circle then head towards centre
        if(from.magnitude >= Radius)
        {
            float angleToCentre = Mathf.Atan2(-from.y, -from.x) * Mathf.Rad2Deg;
            minDegrees = angleToCentre - 45;
            maxDegrees = angleToCentre + 45;
        }


        float angle = Random.Range(minDegrees, maxDegrees) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2( Mathf.Cos(angle), Mathf.Sin(angle));
        float dist = Random.Range(25, 75);
        Vector2 point = from + (dir * dist);

        //Check if point passes through inner circle and outer circle
        if(from.magnitude > InnerRadius && GetCircleIntersection(InnerRadius, from, dir, out Vector2 innerIntersection))
        {
            if((innerIntersection - from).sqrMagnitude < (point - from).sqrMagnitude)
                point = innerIntersection;
        }
        if(point.magnitude > radius && GetCircleIntersection(radius, from, dir, out Vector2 outerItersection))
        {
            if((outerItersection - from).sqrMagnitude < (point - from).sqrMagnitude)
                point = outerItersection;
        }

        return point;
    }

    public void SpawnAnimalsFromCrate(AnimalController.AnimalDef animalDef, int numToSpawn, Vector3 spawnPoint)
    {
        CrateDrop clone = Instantiate<CrateDrop>(crateDropTemplate, islandObjectsContainer.transform);
        clone.gameObject.SetActive(true);
        clone.Drop(animalDef, numToSpawn, spawnPoint, FinishedCrateFall);
    }

    public void FinishedCrateFall(AnimalController.AnimalDef animalDef, int numToSpawn, Vector3 spawnPos)
    {
        AnimalController.Instance.SpawnMultipleAtPosition(animalDef, spawnPos, numToSpawn);
    }

    void Update()
    {
        SortIslandObjects();
    }

    void SortIslandObjects()
    {
        Billboard[] allChildren = islandObjectsContainer.GetComponentsInChildren<Billboard>();
        List<IslandObjectSortDistance> distanceFromCamera = new List<IslandObjectSortDistance>();

        for(int i = 0; i < allChildren.Length; i++)
        {
            RectTransform child = allChildren[i].GetComponent<RectTransform>();
            float distance = Vector3.Distance(child.position, Camera.main.transform.position);
            distanceFromCamera.Add(new IslandObjectSortDistance(child, distance));
        }

        distanceFromCamera.Sort(SortByDistance);

        for(int j = 0; j < distanceFromCamera.Count; j++)
        {
            distanceFromCamera[j].Child.SetSiblingIndex(j);
        }
    }

    int SortByDistance(IslandObjectSortDistance A, IslandObjectSortDistance B)
    {
        return B.Distance.CompareTo(A.Distance);
    }

    struct IslandObjectSortDistance
    {
        public RectTransform Child;
        public float Distance;

        public IslandObjectSortDistance(RectTransform child, float distance)
        {
            Child = child;
            Distance = distance;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, InnerRadius);
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}