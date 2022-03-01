using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerPlaceIncrease : MonoBehaviourPun
{
    private Text placeText;
    private int numberPlace;
    private GameManager gameManager;

    void Start()
    {
        placeText = GetComponent<Text>();
        gameManager = GameManager.instance;
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnIncreaseNumberPlace;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnIncreaseNumberPlace;
    }
    private void OnIncreaseNumberPlace(EventData photonEvent)
    {

        if (photonEvent.Code == (byte)GameFInishedEnum.CurrentStateGame.whoWinGame)
        {
            object[] data = (object[])photonEvent.CustomData;
            string nickPlayer = (string)data[0];
            int myID = (int)data[1];

            if(myID == photonView.ViewID)
            {
                numberPlace = gameManager.numberPlace++;

                placeText.text = $" {nickPlayer} took place {numberPlace} ";
            }
                  
        }
        if(photonEvent.Code == (byte)GameFInishedEnum.CurrentStateGame.gameIsOver)
        {
            if(photonView.IsMine)
                StartCoroutine(gameManager.SetActiveRestartLobby());
        }
        
    }
}
