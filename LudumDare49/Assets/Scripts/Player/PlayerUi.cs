using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : Singleton<PlayerUi>
{
    [SerializeField] Heart lifeIcon;
    [SerializeField] Text score;
    public Image m_extinctAnimalImage;
    public CanvasGroup m_extinctionCanvasGroup;

    private Player player;
    private float previousLife;

    Vector3 m_basePos;
    public Vector3 m_extinctionAnimOffset;

    public override void Awake()
    {
        base.Awake();
        m_basePos = m_extinctionCanvasGroup.transform.localPosition;
    }

    void Start()
    {
        m_extinctionCanvasGroup.gameObject.SetActive(false);
    }

    public void Reset()
    {
        player = null;
        lifeIcon.UpdateFill(1);
        m_extinctionCanvasGroup.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null)
        {
            player = Game.Instance.player;
            previousLife = player.Life;
        }

        score.text = player.CurrentScore.ToString();

        float currentHealth = player.Life;
        if (currentHealth != previousLife)
        {
            float prop = Mathf.InverseLerp(0, Game.Instance.StartingLife, currentHealth);
            lifeIcon.UpdateFill(prop);

            if(currentHealth < previousLife)
            {
                m_extinctAnimalImage.sprite = player.LastExtinction.m_visual.m_sprite;
                m_extinctionCanvasGroup.alpha = 1;
                m_extinctionCanvasGroup.transform.localPosition = m_basePos;
                m_extinctionCanvasGroup.gameObject.SetActive(true);
            }

            previousLife = currentHealth;
        }
        else
        {
            if(m_extinctionCanvasGroup.alpha > 0)
            {
                m_extinctionCanvasGroup.alpha = Mathf.MoveTowards(m_extinctionCanvasGroup.alpha, 0, Time.deltaTime * 0.5f);                
                m_extinctionCanvasGroup.transform.localPosition = Vector3.Lerp(m_basePos + m_extinctionAnimOffset, m_basePos, m_extinctionCanvasGroup.alpha);

                if(m_extinctionCanvasGroup.alpha == 0)
                    m_extinctionCanvasGroup.gameObject.SetActive(false);
            }
        }
    }
}
