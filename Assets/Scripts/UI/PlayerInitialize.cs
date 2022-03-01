using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class PlayerInitialize : MonoBehaviour
{ 
    private int playerID;

    [SerializeField] Text namePlayer;

    public void InitializePlayer(int playerID, string namePlayer)
    {
        this.playerID = playerID;
        this.namePlayer.text = namePlayer;

       

        ExitGames.Client.Photon.Hashtable isReadyProperties = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_IS_READY, false} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(isReadyProperties);

    }
}
