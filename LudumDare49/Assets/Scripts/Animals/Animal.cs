using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    float m_hunger;

    const float HUNGER_EAT = 5;
    const float HUNGER_DIE = 10;

    Vector2 m_from;
    Vector2 m_target;
    float m_speed = 50;

    bool m_wait;
    float m_waitTime;

    public enum eState
    {
        Wait,
        Explore
    }

    float m_bobScale;
    const float DIST_PER_BOB = 30;


    void Update()
    {
        if(m_wait)
        {
            m_waitTime -= Time.deltaTime;
            if(m_waitTime <= 0)
                m_wait = false;

            return;
        }


        Vector2 toTarget = m_target - (Vector2)transform.localPosition;
        float moveAmount = m_speed * Time.deltaTime;

        if(toTarget.sqrMagnitude > moveAmount)
        {
            Vector2 from = (Vector2)transform.localPosition;
            Vector3 pos = Vector2.MoveTowards(from, m_target, moveAmount);

            float distAlong = Vector2.Dot((Vector2)pos - m_from, (m_target - m_from).normalized);
            float bobProgress = distAlong / m_bobScale;

            pos.z = -10 * Mathf.Abs(Mathf.Sin(bobProgress * Mathf.PI));

            transform.localPosition = pos;
        }
        else
        {
            transform.localPosition = m_target;
            m_from = m_target;
            float angle = Random.Range(0, 360) * Mathf.Rad2Deg;
            float radius = Random.Range(0, 200);
            m_target = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
            m_wait = true;
            m_waitTime = 1;

            transform.localScale = new Vector3(Mathf.Sign(m_target.x - m_from.x), 1, 1);

            int numBobs = Mathf.RoundToInt((m_target - m_from).magnitude / DIST_PER_BOB);
            m_bobScale = (m_target - m_from).magnitude / numBobs;
        }


        //bob around

        //look for food

        //eat target

        //look for mate

        //mate




        //
        
    }
}
