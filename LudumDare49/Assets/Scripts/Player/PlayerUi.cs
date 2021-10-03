using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] Heart lifeTemplate;
    [SerializeField] Transform lifeParent;
    [SerializeField] GameObject extinctionLabel;

    private List<Heart> lives = new List<Heart>();

    private Player player;
    private float previousLife;

    void Start()
    {
        float startingLife = Game.Instance.StartingLife;
        for (int i = 0; i < startingLife; i++)
        {
            Heart clone = Instantiate<Heart>(lifeTemplate, lifeParent);
            clone.UpdateFill(1f);
            clone.gameObject.SetActive(true);
            lives.Add(clone);
        }

        player = Game.Instance.player;    
    }

    void Update()
    {
        if (player == null)
        {
            player = Game.Instance.player;
            previousLife = player.Life;
        }

        float currentHealth = player.Life;
        if (currentHealth != previousLife)
        {
            for (int i = 0; i < lives.Count; i++)
            {
                Heart lifeVisual = lives[i];

                float prop = Mathf.Clamp(currentHealth, i, i+1) - i;
                lifeVisual.UpdateFill(prop);
                
            }
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
