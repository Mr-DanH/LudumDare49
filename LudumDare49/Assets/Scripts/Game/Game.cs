using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : Singleton<Game>
{
    static readonly string Player_Score_Key = "PlayerScoreKey";

    [SerializeField] float startingLife;
    [SerializeField] EndGamePopup endGamePopup;

    public float StartingLife { get { return startingLife; } }

    public Player player { get; private set; }

    Dictionary<AnimalController.AnimalDef, int> lastKnownSpecies;

    void Start()
    {
        Init();
    }

    public void Reset()
    {
        Init();
        AnimalController.Instance.Reset();
        OrdersManager.Instance.Reset();
        Island.Instance.Reset();
        ConveyorBelt.Instance.Reset();
        PlayerUi.Instance.Reset();
        
        lastKnownSpecies.Clear();
    }

    void Init()
    {
        player = new Player(startingLife);
        Dictionary<AnimalController.AnimalDef, int> animals = AnimalController.Instance.GatherAnimalIntel(true);
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
            int savedHighScore = PlayerPrefs.GetInt(Player_Score_Key, 0);
            if (savedHighScore < player.CurrentScore)
            {
                PlayerPrefs.SetInt(Player_Score_Key, player.CurrentScore);
                PlayerPrefs.Save();

                savedHighScore = player.CurrentScore;
            }

            endGamePopup.Open(savedHighScore, player.CurrentScore);
        }
        else
        {
            float numExtinctions = 0;
            Dictionary<AnimalController.AnimalDef, int> animals = AnimalController.Instance.GatherAnimalIntel(true);
            AnimalController.AnimalDef extinctAnimal = null;
            foreach(var animal in lastKnownSpecies)
            {
                animals.TryGetValue(animal.Key, out int amount);

                if(amount == 0 && animal.Value > 0)
                {
                    numExtinctions++;
                    extinctAnimal = animal.Key;
                }
            }
            lastKnownSpecies.Clear();
            lastKnownSpecies = new Dictionary<AnimalController.AnimalDef, int>(animals);

            // todo - balance this
            if(numExtinctions > 0)
            {
                player.DecreaseLife(extinctAnimal, numExtinctions/2);
            }
        }
    }
}
