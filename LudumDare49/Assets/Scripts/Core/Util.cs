using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Util
{
    public static List<AnimalController.AnimalDef> PopulateAnimalQueue()
    {
        List<AnimalController.AnimalDef> defs = AnimalController.Instance.AnimalDefs;
        List<AnimalController.AnimalDef> queue = new List<AnimalController.AnimalDef>();
        queue.AddRange(defs);
        queue.AddRange(defs);

        queue.Shuffle();
        
        return queue;
    }

    static System.Random rg = new System.Random();
    static void Shuffle<T>(this IList<T> list)
    {
        int index = list.Count;
        while (index > 1)
        {
            index--;
            int nextIndex = rg.Next(index + 1);
            T value = list[nextIndex];
            list[nextIndex] = list[index];
            list[index] = value;
        }
    }
}
