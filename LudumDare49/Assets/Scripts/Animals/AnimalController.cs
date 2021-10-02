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
        //todo: traits
    }
    List<AnimalDef> m_animalDefs = new List<AnimalDef>();

    void Start()
    {
        foreach(var visual in m_animalVisuals)
        {
            var def = new AnimalDef();
            def.m_visual = visual;
            m_animalDefs.Add(def);
        }

        SpawnAtPosition(m_animalDefs[0], Vector2.zero);
        SpawnAtPosition(m_animalDefs[0], Vector2.zero);
        SpawnAtPosition(m_animalDefs[1], Vector2.zero);
        SpawnAtPosition(m_animalDefs[1], Vector2.zero);
    }

    public void SpawnMultipleAtPosition(AnimalDef def, Vector2 pos, int num)
    {
        for (int i = 0; i < num; i++)
        {
            SpawnAtPosition(def, pos);
        }
    }

    public Animal SpawnAtPosition(AnimalDef def, Vector2 pos)
    {
        var animal = Instantiate(m_prefab, pos, Quaternion.identity, m_island);
        animal.name = m_index++.ToString();
        animal.Init(def);

        m_animals.Add(animal);

        return animal;
    }

    public AnimalDef GetRandomAnimalDef()
    {
        int index = Random.Range(0, m_animalDefs.Count);
        return m_animalDefs[index];
    }

    public Animal FindMate(Animal source)
    {
        List<Animal> animals = m_animals.FindAll(a => a != source && a.CanMate() && a.Def == source.Def);

        if(animals.Count == 0)
            return null;

        return animals[0];
    }
}
