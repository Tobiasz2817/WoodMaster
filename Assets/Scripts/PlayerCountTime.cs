using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;   

public class PlayerCountTime : MonoBehaviourPun
{ 
    private GameManager gameManager;
    private PlayerManager playerManager;
    private float startCounting = 0;

    [SerializeField] Text playerTimerText;
    void Start()
    {
        gameManager = GameManager.instance;
        playerManager = GetComponent<PlayerManager>();
        playerTimerText.text = "Time: " + 0;
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine || !gameManager.isGameStart || playerManager.playerGameOver)
            return;

        photonView.RPC("StartCounting", RpcTarget.All, startCounting);

        startCounting += Time.deltaTime;
    }
    [PunRPC]
    private void StartCounting(float startCounting)
    {
        playerTimerText.text = "Time: " + System.Math.Round(startCounting, 1); 
    }
}
