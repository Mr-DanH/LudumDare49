using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : Singleton<AnimalController>
{
    public Animal m_prefab;
    public Plant m_plantPrefab;
    public Transform m_island;

    [System.Serializable]
    public class AnimalVisual
    {
        public string m_name;
        public Sprite m_sprite;
    }
    public List<AnimalVisual> m_animalVisuals = new List<AnimalVisual>();

    public AnimalVisual m_plantVisual = new AnimalVisual();

    List<Animal> m_animals = new List<Animal>();
    List<Plant> m_plants = new List<Plant>();
    int m_index;

    public class AnimalDef
    {
        public AnimalVisual m_visual;
        public int m_foodWebIndex;
        //todo: traits
    }
    List<AnimalDef> m_animalDefs = new List<AnimalDef>();
    AnimalDef m_plantDef;
    
    [System.Serializable]
    public class FoodWeb
    {
        public List<int> m_eats;
    }
    public List<FoodWeb> m_foodWeb = new List<FoodWeb>();

    void Start()
    {
        Init();
    }

    void Init()
    {
        List<AnimalVisual> visuals = new List<AnimalVisual>(m_animalVisuals);

        for(int i = 0; i < m_foodWeb.Count; ++i)
        {
            var def = new AnimalDef();
            int visualIndex = Random.Range(0, visuals.Count);
            def.m_visual = visuals[visualIndex];
            visuals.RemoveAt(visualIndex);
            def.m_foodWebIndex = i;
            m_animalDefs.Add(def);
        }
    }

    public void Reset()
    {
        DespawnEverything();
        Init();
    }

    public Dictionary<AnimalDef, int> GatherAnimalIntel()
    {
        Dictionary<AnimalDef, int> intel = new Dictionary<AnimalDef, int>();
        foreach (var def in m_animalDefs)
        {
            List<Animal> animalsOfDef = m_animals.FindAll(x => x.Def == def && x.IsFree());
            intel.Add(def, animalsOfDef.Count);
        }

        return intel;
    }

    int currentCollectCountdown = 0;
    public void CollectOrder(Order order, Vector2 portPos, AnimalDef def, int numToCollect)
    {
        currentCollectCountdown = numToCollect;
        List<Animal> animalsOfDef = m_animals.FindAll(x => x.Def == def && x.IsFree());
        for (int i = 0; i < numToCollect; i++)
        {
            animalsOfDef[i].Collect(order, portPos);
        }
    }

    public void SpawnMultipleAtPosition(AnimalDef def, Vector3 pos, int num)
    {
        //use double num so we have buffer ranges between each used range
        float degreesPerSpawn = 360f / (num * 2);

        if(num == 1)
            degreesPerSpawn = 360f;

        for (int i = 0; i < num; i++)
        {
            SpawnAtPosition(def, pos, (i * 2) * degreesPerSpawn, ((i * 2) + 1) * degreesPerSpawn);
        }
    }

    public Animal SpawnAtPosition(AnimalDef def, Vector3 pos, float minDegrees = 0, float maxDegrees = 360)
    {
        var animal = Instantiate(m_prefab, pos, Quaternion.identity, m_island);
        animal.name = m_index++.ToString();
        animal.Init(def, minDegrees, maxDegrees);

        m_animals.Add(animal);

        return animal;
    }

    public Plant SpawnPlantAtPosition(Vector3 pos)
    {
        if(m_plants.Count >= 500)
            return null;

        //Check for overcrowding
        foreach(var otherPlant in m_plants)
        {
            Vector3 diff = otherPlant.transform.position - pos;
            if(diff.sqrMagnitude < 100)
                return null;
        }

        var plant = Instantiate(m_plantPrefab, pos, Quaternion.identity, m_island);
        plant.name = m_index++.ToString();
        m_plants.Add(plant);

        return plant;
    }

    public AnimalDef GetRandomAnimalDef()
    {
        int index = Random.Range(0, m_animalDefs.Count);
        return m_animalDefs[index];
    }

    public List<AnimalDef> AnimalDefs { get { return m_animalDefs; } }

    public T GetClosest<T>(Vector3 worldPos, List<T> sourceList) where T : MonoBehaviour
    {
        T closestAnimal = null;
        float closestDistSq = float.MaxValue;

        foreach(var animal in sourceList)
        {
            float distSq = (worldPos - animal.transform.position).sqrMagnitude;
            if(distSq < closestDistSq)
            {
                closestDistSq = distSq;
                closestAnimal = animal;
            }
        }

        return closestAnimal;
    }

    public Animal FindMate(Animal source)
    {
        List<Animal> animals = m_animals.FindAll(a => a != source && a.CanMate() && a.Def == source.Def);

        if(animals.Count == 0)
            return null;

        animals.RemoveAll(a => Vector2.Angle(source.transform.localPosition, a.transform.localPosition) > 60);

        return GetClosest(source.transform.position, animals);
    }

    public bool IsCarnivore(Animal source)
    {
        return m_foodWeb[source.Def.m_foodWebIndex].m_eats.Count > 0;
    }
    
    public Animal FindPrey(Animal source)
    {
        List<AnimalDef> defs = m_foodWeb[source.Def.m_foodWebIndex].m_eats.ConvertAll(a => m_animalDefs[a]);

        List<Animal> animals = m_animals.FindAll(a => defs.Contains(a.Def) && a.IsFree());

        if(animals.Count == 0)
            return null;
            
        animals.RemoveAll(a => Vector2.Angle(source.transform.localPosition, a.transform.localPosition) > 60);

        return GetClosest(source.transform.position, animals);
    }

    public Plant FindPlant(Animal source)
    {
        List<Plant> plants = m_plants.FindAll(a => a.Scale == 1);

        if(plants.Count == 0)
            return null;
            
        plants.RemoveAll(a => Vector2.Angle(source.transform.localPosition, a.transform.localPosition) > 60);

        return GetClosest(source.transform.position, plants);
    }

    public void Despawn(Animal animal)
    {
        m_animals.Remove(animal);
        Destroy(animal.gameObject);
    }
    public void Despawn(Plant plant)
    {
        m_plants.Remove(plant);
        Destroy(plant.gameObject);
    }

    public void GetPlants(List<Plant> list)
    {
        list.AddRange(m_plants);
    }

    void DespawnEverything()
    {
        for(int i = 0; i < m_animals.Count; i++)
        {
            Destroy(m_animals[i].gameObject);
        }
        m_animals.Clear();

        for(int i = 0; i < m_plants.Count; i++)
        {
            Destroy(m_plants[i].gameObject);
        }
        m_plants.Clear();
    }
}
