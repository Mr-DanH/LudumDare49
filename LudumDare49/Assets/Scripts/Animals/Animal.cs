using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public float MateTime { get; private set; } = 5;

    float m_hunger;

    const float HUNGER_EAT = 5;
    const float HUNGER_DIE = 10;

    Vector2 m_from;
    Vector2 m_target;
    float m_speed = 50;

    public enum eState
    {
        Wait,
        Explore,
        ApproachMate,
        Mate
    }
    public eState State { get; private set; }
    float m_timeRemaining;

    float m_bobScale;
    const float DIST_PER_BOB = 30;

    public Animal Mate { get; set;}
    public Vector2 Target { get { return m_target; } }

    public float Scale { get; set; } = 1;

    public AnimalController.AnimalDef Def { get; private set; }

    public void Init(AnimalController.AnimalDef def)
    {
        Def = def;

        if(TryGetComponent(out Image image))
            image.sprite = def.m_visual.m_sprite;
    }

    public bool CanMate()
    {
        if(MateTime > 0)
            return false;

        if(Mate != null)
            return false;

        switch(State)
        {
            case eState.ApproachMate:
            case eState.Mate:
                return false;
        }

        return true;
    }

    void MoveTo(Vector2 target)
    {
        m_from = transform.localPosition;
        m_target = target;

        transform.localScale = new Vector3(Mathf.Sign(m_from.x - m_target.x), 1, 1);

        int numBobs = Mathf.RoundToInt((m_target - m_from).magnitude / DIST_PER_BOB);
        m_bobScale = (m_target - m_from).magnitude / numBobs;
    }

    bool MoveTowardsTarget()
    {
        Vector2 toTarget = m_target - (Vector2)transform.localPosition;
        float moveAmount = m_speed * Time.deltaTime;

        if(toTarget.sqrMagnitude < moveAmount)
        {
            transform.localPosition = m_target;
            return true;
        }

        Vector2 from = (Vector2)transform.localPosition;
        Vector3 pos = Vector2.MoveTowards(from, m_target, moveAmount);

        float distAlong = Vector2.Dot((Vector2)pos - m_from, (m_target - m_from).normalized);
        float bobProgress = distAlong / m_bobScale;

        pos.z = -10 * Mathf.Abs(Mathf.Sin(bobProgress * Mathf.PI));

        transform.localPosition = pos;

        return false;
    }

    void MoveToMate()
    {
        bool isLeft = Mate.Target.x > m_target.x;
        float xOffset = isLeft ? -40 : 40;
        MoveTo(Mate.Target + new Vector2(xOffset, 0));
        transform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1) * Scale;
        State = eState.ApproachMate;
    }

    void Update()
    {
        if(Scale < 1)
        {
            Scale = Mathf.MoveTowards(Scale, 1, Time.deltaTime * 0.25f);
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(Mathf.Sign(scale.x), Mathf.Sign(scale.y), Mathf.Sign(scale.z)) * Scale;
        }

        MateTime = Mathf.MoveTowards(MateTime, 0, Time.deltaTime);

        switch(State)
        {
            case eState.Wait:
                {
                    if(Mate != null)
                    {
                        MoveToMate();
                        return;
                    }

                    m_timeRemaining -= Time.deltaTime;
                    if(m_timeRemaining < 0)
                    {
                        m_from = transform.localPosition;

                        if(MateTime == 0)
                        {
                            var mate = AnimalController.Instance.FindMate(this);
                            if (mate != null) 
                            {
                                Mate = mate;
                                Mate.Mate = this;
                                // Debug.LogWarning("from " + this + " " + transform.GetSiblingIndex(), this);
                                // Debug.LogWarning("to " + Mate + " " + Mate.transform.GetSiblingIndex(), Mate);

                                MoveToMate();
                                return;
                            }
                        }

                        float angle = Random.Range(0, 360) * Mathf.Rad2Deg;
                        float radius = Random.Range(0, 200);

                        MoveTo(new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)));
                        State = eState.Explore;
                    }                    
                }
                break;

            case eState.Explore:
                {
                    if(MoveTowardsTarget())
                    {
                        if(Mate != null)
                        {
                            MoveToMate();
                            return;
                        }

                        m_timeRemaining = 1;
                        State = eState.Wait;
                    }
                }
                break;

            case eState.ApproachMate:
                {
                    if(MoveTowardsTarget() && (Vector2)Mate.transform.localPosition == Mate.Target)
                    {
                        m_timeRemaining = 1;
                        State = eState.Mate;
                    }
                }
                break;

            case eState.Mate:
                {
                    m_timeRemaining -= Time.deltaTime;

                    Vector3 pos = m_target;

                    pos.z = 10 * Mathf.Abs(Mathf.Sin((m_timeRemaining * 4) * Mathf.PI));

                    transform.localPosition = pos;

                    if(m_timeRemaining < 0)
                    {
                        if(Mate != null)
                        {
                            var baby = AnimalController.Instance.SpawnAtPosition(Def, (transform.position + Mate.transform.position) / 2);
                            baby.Scale = 0.25f;
                            baby.transform.localScale = Vector3.one * baby.Scale;
                            //    Debug.Log("baby " + baby + " " + baby.transform.GetSiblingIndex(), this);

                            // if(Mate.State != eState.Mate)
                            // {
                            //     Debug.LogError(this, this);
                            //     Debug.LogError(Mate, Mate);
                            //     return;
                            // }

                            Mate.Mate = null;
                            Mate = null;
                        }
                        
                        float angle = Random.Range(0, 360) * Mathf.Rad2Deg;
                        float radius = Random.Range(0, 200);

                        MoveTo(new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)));
                        State = eState.Explore;
                    }                    
                }
                break;
        }
    }
}
