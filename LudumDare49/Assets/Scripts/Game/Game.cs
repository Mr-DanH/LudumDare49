using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
{
    static readonly string Player_Score_Key = "PlayerScoreKey";

    [SerializeField] float startingLife;

    public float StartingLife { get { return startingLife; } }

    public Player player { get; private set; }

    Dictionary<AnimalController.AnimalDef, int> lastKnownSpecies;

    void Start()
    {
        player = new Player(startingLife);
        Dictionary<AnimalController.AnimalDef, int> animals = AnimalController.Instance.GatherAnimalIntel();
        lastKnownSpecies = new Dictionary<AnimalController.AnimalDef, int>(animals);
    }

    public void IncreaseScore(int points)
    {
        player.IncreaseScore(points);
    }

    void Update()
    {
        if (!player.HasLife())
        {
            // todo - end menu
            int savedHighScore = PlayerPrefs.GetInt(Player_Score_Key);
            if (!PlayerPrefs.HasKey(Player_Score_Key) || savedHighScore < player.CurrentScore)
            {
                PlayerPrefs.SetInt(Player_Score_Key, player.CurrentScore);
                PlayerPrefs.Save();
            }
        }
        else
        {
            float numExtinctions = 0;
            Dictionary<AnimalController.AnimalDef, int> animals = AnimalController.Instance.GatherAnimalIntel();
            foreach(var animal in lastKnownSpecies)
            {
                animals.TryGetValue(animal.Key, out int amount);

                if(amount == 0 && animal.Value > 0)
                {
                    numExtinctions++;
                }
            }
            lastKnownSpecies.Clear();
            lastKnownSpecies = new Dictionary<AnimalController.AnimalDef, int>(animals);

            // todo - balance this
            if(numExtinctions > 0)
            {
                player.DecreaseLife(numExtinctions/2);
            }
        }
    }
}
