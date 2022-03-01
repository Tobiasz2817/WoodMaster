using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class StartGameCounting : MonoBehaviourPun
{
    private TextMeshProUGUI countTime;
    private GameObject playerUI;
    private GameManager gameManager;

    [SerializeField] int startTime = 8;

    void Start()
    {
        gameManager = GameManager.instance;

        countTime = gameManager.countTime;
        playerUI = gameManager.playerUI;

        if (photonView.IsMine)
            playerUI.SetActive(true);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartCoroutine(ICountTime());
        }
    }
    IEnumerator ICountTime()
    {
        for (int i = startTime; i >= 0; i--)
        {
            yield return new WaitForSeconds(1);
            photonView.RPC("SendTimeToServer", RpcTarget.AllBuffered, i);
        }
    }
    [PunRPC]
    private void SendTimeToServer(int counter)
    {
        countTime.text = counter.ToString();

        if (counter == 0)
        {
            gameManager.isGameStart = true;
            playerUI.SetActive(false);
        }
    }
}
