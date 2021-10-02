using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public Image m_image;

    public float MateTime { get; private set; } = MATE_TIME;

    float m_hunger;

    const float HUNGER_EAT = 5;
    const float HUNGER_DIE = 15;
    const float MATE_TIME = 10;

    Vector2 m_from;
    Vector2 m_target;
    float m_speed = 50;

    public enum eState
    {
        Wait,
        Explore,
        ApproachMate,
        Mate,
        Grazing,
        Chasing,
        Dead
    }
    public eState State { get; private set; }
    float m_timeRemaining;

    float m_bobScale;
    const float DIST_PER_BOB = 30;

    public Animal Mate { get; set;}
    public Animal Prey { get; set;}
    public Vector2 Target { get { return m_target; } }

    public float Scale { get; set; } = 1;

    public AnimalController.AnimalDef Def { get; private set; }

    public void Init(AnimalController.AnimalDef def, float minDegrees, float maxDegrees)
    {
        Def = def;
        m_image.sprite = def.m_visual.m_sprite;
        
        MoveTo(Island.Instance.GetRandomMoveTarget(minDegrees, maxDegrees));
        State = eState.Explore;
    }

    public bool CanMate()
    {
        if(MateTime > 0)
            return false;

        if(Mate != null)
            return false;

        switch(State)
        {
            case eState.Wait:
            case eState.Explore:
                return true;
        }

        return false;
    }

    void MoveTo(Vector2 target)
    {
        m_from = transform.localPosition;
        m_target = target;

        transform.localScale = new Vector3(Mathf.Sign(m_from.x - m_target.x), 1, 1);

        int numBobs = Mathf.RoundToInt((m_target - m_from).magnitude / DIST_PER_BOB);
        m_bobScale = (m_target - m_from).magnitude / numBobs;
    }

    bool MoveTowardsTarget(float speedFactor = 1)
    {
        Vector2 toTarget = m_target - (Vector2)transform.localPosition;
        float moveAmount = speedFactor * m_speed * Time.deltaTime;

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

    public void Kill()
    {
        gameObject.AddComponent<CanvasGroup>();
        Vector3 scale = transform.localScale;
        m_image.transform.localScale = new Vector3(m_image.transform.localScale.x, -1, 1);
        m_timeRemaining = 1;
        State = eState.Dead;
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
        m_hunger += Time.deltaTime;

        //hunger checks

        switch(State)
        {
            case eState.Wait:
                {
                    if(Mate != null)
                    {
                        MoveToMate();
                        return;
                    }

                    if(m_hunger > HUNGER_DIE && AnimalController.Instance.IsCarnivore(this))
                    {
                        Kill();
                        return;
                    }

                    if(m_hunger > HUNGER_EAT)
                    {
                        Animal prey = AnimalController.Instance.FindPrey(this);
                        if(prey != null)
                        {
                            Prey = prey;
                            MoveTo(prey.transform.localPosition);
                            State = eState.Chasing;
                            return;
                        }
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
                                MoveToMate();
                                return;
                            }
                        }

                        MoveTo(Island.Instance.GetRandomMoveTarget());
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

                            Mate.Mate = null;
                            Mate = null;
                        }

                        MateTime = MATE_TIME;
                        
                        float angle = Random.Range(0, 360) * Mathf.Rad2Deg;
                        float radius = Random.Range(0, 200);

                        MoveTo(new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)));
                        State = eState.Explore;
                    }                    
                }
                break;

            case eState.Chasing:
                {
                    if(Prey == null || Prey.State == eState.Dead)
                    {
                        m_timeRemaining = 1;
                        State = eState.Wait;
                        return;
                    }

                    m_target = Prey.transform.localPosition;
                    if(MoveTowardsTarget(2))
                    {
                        Prey.Kill();
                        m_hunger = 0;

                        m_timeRemaining = 1;
                        State = eState.Wait;
                    }
                }
                break;                

            case eState.Dead:
                {
                    Vector3 pos = transform.localPosition;
                    pos.z -= Time.deltaTime * 50;
                    transform.localPosition = pos;

                    m_timeRemaining -= Time.deltaTime;
                    gameObject.GetComponent<CanvasGroup>().alpha = m_timeRemaining;
                    
                    if(m_timeRemaining <= 0)
                        AnimalController.Instance.Despawn(this);
                }
                break;
        }
    }
}
