using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : Singleton<PlayerUi>
{
    [SerializeField] Heart lifeIcon;
    [SerializeField] Text score;
    [SerializeField] GameObject extinctionLabel;

    private Player player;
    private float previousLife;    
    float extinctionTimer = 0f;

    public void Reset()
    {
        player = null;
        lifeIcon.UpdateFill(1);
        extinctionTimer = 0f;
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
            extinctionTimer = 1f;
            extinctionLabel.SetActive(true);
            previousLife = currentHealth;
        }

        extinctionTimer -= Time.deltaTime;

        if (extinctionTimer <= 0)
        {
            extinctionLabel.SetActive(false);
        }
    }
}
