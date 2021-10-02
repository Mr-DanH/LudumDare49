using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDrop : MonoBehaviour
{
    [SerializeField] AnimationCurve fallCurve;
    [SerializeField] float startingHeight;
    [SerializeField] float duration;

    float timer;
    System.Action<AnimalController.AnimalDef, int, Vector3> callback;
    AnimalController.AnimalDef m_AnimalDef;
    int m_NumToSpawn;
    Vector3 m_SpawnPos;

    public void Drop(AnimalController.AnimalDef animalDef, int numToSpawn, Vector3 spawnPoint, System.Action<AnimalController.AnimalDef, int, Vector3> finishedFalling)
    {
        m_AnimalDef = animalDef;
        m_NumToSpawn = numToSpawn;
        m_SpawnPos = spawnPoint;
        callback = finishedFalling;

        StartCoroutine(CoFall());
    }

    IEnumerator<YieldInstruction> CoFall()
    {
        float t = 0f;
        Vector3 startingPos = m_SpawnPos;
        startingPos.y -= startingHeight; // todo - probably need to change

        while (t < 1)
        {
            timer += Time.deltaTime;
            t = fallCurve.Evaluate(timer/duration);
            transform.position = Vector3.Lerp(startingPos, m_SpawnPos, t);
            yield return null;
        }

        callback.Invoke(m_AnimalDef, m_NumToSpawn, m_SpawnPos);

        Destroy(gameObject);

    }
}
