using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] Heart lifeIcon;
    [SerializeField] Text score;
    [SerializeField] GameObject extinctionLabel;

    private Player player;
    private float previousLife;

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
    
    float extinctionTimer = 0f;
}
