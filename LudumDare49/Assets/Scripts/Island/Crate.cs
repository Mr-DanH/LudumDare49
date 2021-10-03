using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crate : MonoBehaviour
{
    [SerializeField] Image animal;
    [SerializeField] Image spawnNum;
    [SerializeField] GameObject invalid;
    [SerializeField] GameObject parachute;
    [SerializeField] CrateScriptableObject crateScriptableObject;

    public AnimalController.AnimalDef AnimalDef { get; private set; }
    public int NumToSpawn { get; private set; }

    public void Init(int numToSpawn, AnimalController.AnimalDef animalDef, bool useParachute = false)
    {
        AnimalDef = animalDef;
        NumToSpawn = numToSpawn;

        animal.sprite = AnimalDef.m_visual.m_sprite;
        spawnNum.sprite = crateScriptableObject.GetCrateNum(numToSpawn);

        parachute.SetActive(useParachute);
        invalid.SetActive(false);
        gameObject.SetActive(true);
    }

    public void SetInvalid(bool isInvalid)
    {
        invalid.SetActive(isInvalid);
    }
}
