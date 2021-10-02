using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : Singleton<AnimalController>
{
    public Animal m_prefab;
    public Transform m_island;

    [System.Serializable]
    public class AnimalVisual
    {
        public string m_name;
        public Sprite m_sprite;
    }
    public List<AnimalVisual> m_animalVisuals = new List<AnimalVisual>();

    List<Animal> m_animals = new List<Animal>();
    int m_index;

    public class AnimalDef
    {
        public AnimalVisual m_visual;
        public int m_foodWebIndex;
        //todo: traits
    }
    List<AnimalDef> m_animalDefs = new List<AnimalDef>();
    
    [System.Serializable]
    public class FoodWeb
    {
        public List<int> m_eats;
    }
    public List<FoodWeb> m_foodWeb = new List<FoodWeb>();

    void Start()
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

        // SpawnAtPosition(m_animalDefs[0], Vector2.zero);
        // SpawnAtPosition(m_animalDefs[0], Vector2.zero);
        // SpawnAtPosition(m_animalDefs[1], Vector2.zero);
        // SpawnAtPosition(m_animalDefs[1], Vector2.zero);
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

    public AnimalDef GetRandomAnimalDef()
    {
        int index = Random.Range(0, m_animalDefs.Count);
        return m_animalDefs[index];
    }

    public Animal GetClosest(Vector3 worldPos, List<Animal> sourceList)
    {
        Animal closestAnimal = null;
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

        return GetClosest(source.transform.position, animals);
    }

    public bool IsCarnivore(Animal source)
    {
        return m_foodWeb[source.Def.m_foodWebIndex].m_eats.Count > 0;
    }
    
    public Animal FindPrey(Animal source)
    {
        List<AnimalDef> defs = m_foodWeb[source.Def.m_foodWebIndex].m_eats.ConvertAll(a => m_animalDefs[a]);

        List<Animal> animals = m_animals.FindAll(a => defs.Contains(a.Def));

        if(animals.Count == 0)
            return null;

        return GetClosest(source.transform.position, animals);
    }

    public void Despawn(Animal animal)
    {
        m_animals.Remove(animal);
        Destroy(animal.gameObject);
    }
}
