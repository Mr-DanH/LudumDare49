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

    public List<float> m_biomeDivisions = new List<float>();

    class Biome
    {
        public float m_minAngle;
        public float m_maxAngle;
        public float m_fertility;
    }
    List<Biome> m_biomes = new List<Biome>();

    float m_plantCheckTime;

    public override void Awake()
    {
        base.Awake();

        InitAwake();
    }

    void InitAwake()
    {
        List<float> fertilities = new List<float>{0, 0.04f, 0.06f, 0.08f};

        for(int i = 0; i < m_biomeDivisions.Count; ++i)
        {
            Biome biome = new Biome();
            biome.m_minAngle = m_biomeDivisions[i];
            biome.m_maxAngle = m_biomeDivisions[(i + 1) % m_biomeDivisions.Count];

            if(biome.m_maxAngle < biome.m_minAngle)
                biome.m_maxAngle += 360;

            int fertilityIndex = Random.Range(0, fertilities.Count);
            biome.m_fertility = fertilities[fertilityIndex];
            fertilities.RemoveAt(fertilityIndex);

            m_biomes.Add(biome);
        }        
    }

    void Start()
    {
        InitStart();
    }

    void InitStart()
    {
        //Prepopulate some plants
        foreach(var biome in m_biomes)
        {
            for(float i = 0; i < biome.m_fertility; i += 0.02f) 
            {
                float angle = Random.Range(biome.m_minAngle, biome.m_maxAngle) * Mathf.Deg2Rad;
                float radius = Random.Range(Radius * 0.33f, Radius);
                Vector2 localPos = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));

                AnimalController.Instance.SpawnPlantAtPosition(AnimalController.Instance.m_island.transform.TransformPoint(localPos));
            }
        }
    }

    public void Reset()
    {
        InitAwake();
        InitStart();
    }

    bool GetCircleIntersection(float radius, Vector2 from, Vector2 dir, out Vector2 point)
    {
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

    Biome GetBiomeForAngle(float angle)
    {
        foreach(var biome in m_biomes)
        {
            if(biome.m_minAngle <= angle && biome.m_maxAngle >= angle)
                return biome;
        }

        angle += 360;
        foreach(var biome in m_biomes)
        {
            if(biome.m_minAngle <= angle && biome.m_maxAngle >= angle)
                return biome;
        }

        return null;
    }

    void Update()
    {
        m_plantCheckTime -= Time.deltaTime;

        if(m_plantCheckTime < 0)
        {
            foreach(var biome in m_biomes)
            {
                if(Random.value < biome.m_fertility)
                {
                    float angle = Random.Range(biome.m_minAngle, biome.m_maxAngle) * Mathf.Deg2Rad;
                    float radius = Random.Range(Radius * 0.33f, Radius);
                    Vector2 localPos = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));

                    AnimalController.Instance.SpawnPlantAtPosition(AnimalController.Instance.m_island.transform.TransformPoint(localPos));
                }
            }

            List<Plant> plants = new List<Plant>();
            AnimalController.Instance.GetPlants(plants);

            foreach (var plant in plants)
            {
                if(plant.Scale < 1)
                    continue;

                float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                float radius = 20;
                Vector2 localPos = (Vector2)plant.transform.localPosition + new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));

                if(localPos.magnitude <= InnerRadius || localPos.magnitude >= Radius)
                    continue;

                float angleToSpawnPos = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
                var biome = GetBiomeForAngle(angleToSpawnPos);
                
                if(Random.value < biome.m_fertility * 2)
                    AnimalController.Instance.SpawnPlantAtPosition(AnimalController.Instance.m_island.transform.TransformPoint(localPos));
            }

            m_plantCheckTime += 1;
        }

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

        foreach(float division in m_biomeDivisions)
        {
            Vector2 dir = new Vector2( Mathf.Cos(division * Mathf.Deg2Rad), Mathf.Sin(division * Mathf.Deg2Rad));
            Vector3 worldDir = transform.TransformVector(dir);

            Gizmos.DrawLine(worldDir * InnerRadius, worldDir * Radius);
        }
    }

}