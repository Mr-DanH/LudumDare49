using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    float life;
    float currentScore;
    
    public int CurrentScore { get { return (int)currentScore; } }
    public float Life { get { return life; } }
    public AnimalController.AnimalDef LastExtinction { get; set; }

    public bool HasLife()
    {
        return life > 0;
    }

    public Player(float startingLife)
    {
        life = startingLife;
    }

    public void IncreaseScore(int points)
    {
        if (HasLife())
        {
            currentScore += points;
        }
    }

    public void DecreaseLife(AnimalController.AnimalDef def, float amount)
    {
        LastExtinction = def;
        life -= amount;
        life = life < 0 ? 0 : life;
    }
}
