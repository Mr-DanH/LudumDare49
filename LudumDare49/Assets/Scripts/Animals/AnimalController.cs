using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : Singleton<AnimalController>
{
    public Animal m_prefab;
    public Transform m_island;

    List<Animal> m_animals = new List<Animal>();
    int m_index;

    void Start()
    {
        SpawnAtPosition(Vector2.zero);
        SpawnAtPosition(Vector2.zero);        
    }

    public Animal SpawnAtPosition(Vector2 pos)
    {
        var animal = Instantiate(m_prefab, pos, Quaternion.identity, m_island);
        animal.name = m_index++.ToString();

        m_animals.Add(animal);

        return animal;
    }
}
