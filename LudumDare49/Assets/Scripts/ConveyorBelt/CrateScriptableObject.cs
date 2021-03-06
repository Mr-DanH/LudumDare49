using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CrateScriptableObject", order = 1)]
public class CrateScriptableObject : ScriptableObject
{
    [SerializeField] List<Sprite> crateNumberVisuals;
    [SerializeField] List<Sprite> boatNumberVisuals;

    public Sprite GetCrateNum(int num)
    {
        return crateNumberVisuals[num-1];
    }    
    public Sprite GetBoatNum(int num)
    {
        return boatNumberVisuals[num-1];
    }
}
