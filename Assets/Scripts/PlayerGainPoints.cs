using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerGainPoints : MonoBehaviourPun
{
    [SerializeField] Text scoreText;

    private int score = 0;

    private void Start()
    {
            
    }

    [PunRPC]
    public void MultiplyScore(int scoreToIncrease)
    {
        score += scoreToIncrease;

        scoreText.text = "Score: " + score;

    }
}
