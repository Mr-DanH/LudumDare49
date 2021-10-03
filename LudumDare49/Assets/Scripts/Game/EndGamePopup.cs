using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePopup : MonoBehaviour
{
    [SerializeField] Text score;
    [SerializeField] Text highScore;

    public void Open(int savedHighScore, int currentScore)
    {
        gameObject.SetActive(true);
        score.text = currentScore.ToString();
        highScore.text = savedHighScore.ToString();
    }

    public void UiPlayAgain()
    {
        Game.Instance.Reset();
        gameObject.SetActive(false);
    }
}
