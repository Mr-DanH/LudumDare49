using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDrop : MonoBehaviour
{
    [SerializeField] Crate crate;
    [SerializeField] AnimationCurve fallCurve;
    [SerializeField] float startingHeight;
    [SerializeField] float duration;

    float timer;
    System.Action<AnimalController.AnimalDef, int, Vector3> callback;
    Vector3 m_SpawnPos;

    public void Drop(AnimalController.AnimalDef animalDef, int numToSpawn, Vector3 spawnPoint, System.Action<AnimalController.AnimalDef, int, Vector3> finishedFalling)
    {
        crate.Init(numToSpawn, animalDef, useParachute: true);
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

        callback.Invoke(crate.AnimalDef, crate.NumToSpawn, m_SpawnPos);

        Destroy(gameObject);

    }
}
