using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public Image m_image;
    public GameObject m_hungry;
    public GameObject m_old;

    public float MateTime { get; private set; } = MATE_TIME;

    float m_hunger;
    float m_life = 60;

    const float HUNGER_EAT = 5;
    const float HUNGER_THOUGHT = 10;
    const float HUNGER_DIE = 15;
    const float MATE_TIME = 10;
    const float OLD_THOUGHT = 5;

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
        Dead,
        Collecting
    }
    public eState State { get; private set; }
    float m_timeRemaining;

    float m_bobScale;
    const float DIST_PER_BOB = 30;

    public Animal Mate { get; set;}
    public Animal Prey { get; set;}
    public Plant Plant { get; set; }
    public Vector2 Target { get { return m_target; } }

    public float Scale { get; set; } = 1;

    public AnimalController.AnimalDef Def { get; private set; }

    public void Init(AnimalController.AnimalDef def, float minDegrees, float maxDegrees)
    {
        Def = def;
        m_image.sprite = def.m_visual.m_sprite;

        m_hungry.SetActive(false);
        m_old.SetActive(false);
        
        MoveTo(Island.Instance.GetRandomMoveTarget(transform.localPosition, minDegrees, maxDegrees));
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
        Vector3 direction = (Vector3)Mate.Target - transform.localPosition;

        Transform camera = Camera.main.transform;
        Vector3 worldDir = transform.parent.TransformVector(direction);
        Vector3 cameraLocalDir = camera.InverseTransformVector(worldDir);

        Vector3 cameraOffset = new Vector3(-Mathf.Sign(cameraLocalDir.x), 0, 0);

        Vector3 worldOffset = camera.TransformVector(cameraOffset);
        Vector3 localoffset = transform.parent.InverseTransformVector(worldOffset);

        MoveTo(Mate.Target + ((Vector2)localoffset * 40));
        State = eState.ApproachMate;
    }

    public void EndMating()
    {
        Mate = null;
        MateTime = MATE_TIME;                        

        MoveTo(Island.Instance.GetRandomMoveTarget(transform.localPosition));
        State = eState.Explore;
    }

    public void Kill()
    {
        m_hungry.SetActive(false);
        m_old.SetActive(false);

        gameObject.AddComponent<CanvasGroup>();
        Vector3 scale = transform.localScale;
        m_image.transform.localScale = new Vector3(m_image.transform.localScale.x, -1, 1);
        m_timeRemaining = 1;
        State = eState.Dead;
    }

    public void Collect()
    {
        m_hungry.SetActive(false);
        m_old.SetActive(false);

        gameObject.AddComponent<CanvasGroup>();
        m_timeRemaining = 1;
        State = eState.Collecting;
    }

    bool IsFree()
    {
        return State != eState.Dead && State != eState.Collecting;
    }

    void Update()
    {
        if(Scale < 1)
        {
            Scale = Mathf.MoveTowards(Scale, 1, Time.deltaTime * 0.25f);
            transform.localScale = Vector3.one * Scale;
        }

        MateTime = Mathf.MoveTowards(MateTime, 0, Time.deltaTime);
        m_hunger += Time.deltaTime;
        m_life -= Time.deltaTime;

        if(IsFree() && m_hunger > HUNGER_DIE)
        {
            Kill();
            return;
        }

        if(IsFree() && m_life < 0)
        {
            Kill();
            return;
        }

        if(IsFree())
        {
            float timeToStarve = HUNGER_DIE - m_hunger;
            float timeToOldAge = m_life;

            bool showHungry = (m_hunger > HUNGER_THOUGHT);
            bool showOldAge = (m_life < OLD_THOUGHT);

            if(showHungry != showOldAge)
            {
                m_hungry.SetActive(showHungry);
                m_old.SetActive(showOldAge);
            }
            else if (showHungry && showOldAge)
            {                
                m_hungry.SetActive(timeToStarve <= timeToOldAge);
                m_old.SetActive(timeToStarve > timeToOldAge);
            }
            else
            {                
                m_hungry.SetActive(false);
                m_old.SetActive(false);
            }
        }        

        if (IsFree() && State != eState.Wait)
        {
            Transform camera = Camera.main.transform;
            Vector3 direction = (Vector3)m_target - transform.localPosition;
            direction.z = 0;

            if(direction == Vector3.zero && Mate != null)
                direction = (Vector3)Mate.Target - transform.localPosition;

            direction.z = 0;

            Vector3 worldDir = transform.parent.TransformVector(direction);
            Vector3 cameraLocalDir = camera.InverseTransformVector(worldDir);

            m_image.transform.localScale = new Vector3(-Mathf.Sign(cameraLocalDir.x), 1, 1);
        }

        switch(State)
        {
            case eState.Wait:
                {
                    if(Mate != null)
                    {
                        MoveToMate();
                        return;
                    }

                    if(m_hunger > HUNGER_EAT)
                    {
                        if(AnimalController.Instance.IsCarnivore(this))
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
                        else
                        {
                            Plant prey = AnimalController.Instance.FindPlant(this);
                            if(prey != null)
                            {
                                Plant = prey;
                                MoveTo(prey.transform.localPosition);
                                State = eState.Chasing;
                                return;
                            }
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

                        MoveTo(Island.Instance.GetRandomMoveTarget(transform.localPosition));
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
                    if(Mate == null || Mate.State == eState.Dead)
                    {
                        m_timeRemaining = 1;
                        State = eState.Wait;
                        return;
                    }

                    if(MoveTowardsTarget() && (Vector2)Mate.transform.localPosition == Mate.Target)
                    {
                        m_timeRemaining = 1;
                        State = eState.Mate;
                    }
                }
                break;

            case eState.Mate:
                {
                    if(Mate == null || Mate.State == eState.Dead)
                    {
                        m_timeRemaining = 1;
                        State = eState.Wait;
                        return;
                    }

                    m_timeRemaining -= Time.deltaTime;

                    Vector3 pos = m_target;

                    pos.z = 10 * Mathf.Abs(Mathf.Sin((m_timeRemaining * 4) * Mathf.PI));

                    transform.localPosition = pos;

                    if(m_timeRemaining < 0)
                    {
                        var baby = AnimalController.Instance.SpawnAtPosition(Def, (transform.position + Mate.transform.position) / 2);
                        baby.Scale = 0.25f;
                        baby.transform.localScale = Vector3.one * baby.Scale;

                        Mate.EndMating();
                        EndMating();
                    }                    
                }
                break;

            case eState.Chasing:
                {
                    if((Prey == null || Prey.State == eState.Dead) && Plant == null)
                    {
                        m_timeRemaining = 1;
                        State = eState.Wait;
                        return;
                    }

                    if(Prey != null)
                    {
                        m_target = Prey.transform.localPosition;
                        if(MoveTowardsTarget(2))
                        {
                            Prey.Kill();
                            Prey = null;
                            m_hunger = 0;
                            m_hungry.SetActive(false);

                            m_timeRemaining = 1;
                            State = eState.Wait;
                        }
                    }
                    else
                    {
                        if(MoveTowardsTarget(2))
                        {
                            AnimalController.Instance.Despawn(Plant);
                            Plant = null;
                            m_hunger = 0;
                            m_hungry.SetActive(false);

                            m_timeRemaining = 1;
                            State = eState.Wait;                            
                        }
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
            case eState.Collecting:
                {
                    Vector3 pos = transform.localPosition;
                    pos.z -= Time.deltaTime * 50;
                    transform.localPosition = pos;

                    m_timeRemaining -= Time.deltaTime;
                    gameObject.GetComponent<CanvasGroup>().alpha = m_timeRemaining;
                    
                    if(m_timeRemaining <= 0)
                    {
                        AnimalController.Instance.Despawn(this, collected: true);
                    }
                }
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 targetPos = transform.parent.TransformPoint(m_target);
        Gizmos.DrawWireSphere(targetPos, 10);
        Gizmos.DrawLine(transform.position, targetPos);
    }
}
