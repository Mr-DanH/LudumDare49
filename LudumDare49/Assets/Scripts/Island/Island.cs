using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : Singleton<Island>
{
    [SerializeField] GameObject islandObjectsContainer;
    [SerializeField] float radius = 200f;
     public float Radius { get { return radius; } }

    public Vector2 GetRandomMoveTarget(float minDegrees = 0, float maxDegrees = 360)
    {
        float angle = Random.Range(minDegrees, maxDegrees) * Mathf.Deg2Rad;
        float radius = Random.Range(Radius * 0.33f, Radius);
        return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
    }

    void Update()
    {
        SortIslandObjects();
    }

    void SortIslandObjects()
    {
        RectTransform[] allChildren = islandObjectsContainer.GetComponentsInChildren<RectTransform>();
        List<IslandObjectSortDistance> distanceFromCamera = new List<IslandObjectSortDistance>();

        for(int i = 0; i < allChildren.Length; i++)
        {
            RectTransform child = allChildren[i];
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

}