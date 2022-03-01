using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    
    [SerializeField] Text statusNetwork;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject joinButton;

    [SerializeField] int countTable;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;    

        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        joinButton.SetActive(false);
        startButton.SetActive(false);
    }
    private void Update()
    {
        statusNetwork.text = "Aktualny stan: " + PhotonNetwork.NetworkClientState;
    }

    public override void OnConnected()
    {
        Debug.Log("£¹cze siê z serwerem " + this);
    }
    public override void OnConnectedToMaster()
    {
        SetPlayerName();
        Debug.Log("Po³¹czy³em siê z serwerem i mam nickname: " + PhotonNetwork.NickName);
        
        startButton.SetActive(true);
        startButton.GetComponent<Button>().onClick.AddListener(JoinRandomRoom);      
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room: " + Random.Range(1,10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.IsVisible = true;

        // Random Number For Game
      

        string[] randomBranch = { "rb" };
        ExitGames.Client.Photon.Hashtable custonPropertiesGame = new ExitGames.Client.Photon.Hashtable() { { "rb", RandomBranchPos(countTable) } };
        roomOptions.CustomRoomPropertiesForLobby = randomBranch;
        roomOptions.CustomRoomProperties = custonPropertiesGame;

        PhotonNetwork.CreateRoom(roomName,roomOptions);

        Debug.Log("Stworzy³em Pokoj a mój nickname to " + PhotonNetwork.LocalPlayer.NickName);

    }
    public override void OnCreatedRoom()
    {
        
    }
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            joinButton.SetActive(true);
            joinButton.GetComponent<Button>().onClick.AddListener(JoinToScene);
        }

        Debug.Log("Do³¹czy³em do pokoju " + PhotonNetwork.CurrentRoom.Name);
    }
    private int[] RandomBranchPos(int countArray)
    {
        int[] tableNumber = new int[countArray];
        for (int i = 0; i < tableNumber.Length - 2; i++)
        {

            if (i < 1 || i == countArray - 1)
            {
                tableNumber[i] = 0;

                continue;
            }

            while(true)
            {
                tableNumber[i] = Random.Range(-1, 2);

                if (tableNumber[i] != tableNumber[i - 1])
                {
                    if(tableNumber[i] == 0 || tableNumber[i - 1] == 0)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                    
                }
                else if(tableNumber[i] == tableNumber[i - 1])
                {
                    if(tableNumber[i] == 0 && tableNumber[i - 1] == 0)
                    {
                        continue;
                    }
                    else if(tableNumber[i] == tableNumber[i - 1] && tableNumber[i] != tableNumber[i - 2])
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (i > 3)
            {

                // mo¿na dodaæ w przypadku gdy nastêpn¹ liczb¹ mo¿e byæ przeciwna to table[i + 1] == 0 || table[i + 1] == table[i] 
                if ((tableNumber[i - 4] == tableNumber[i - 3] && tableNumber[i - 1] == tableNumber[i] && tableNumber[i - 4] == tableNumber[i - 1] && tableNumber[i - 3] == tableNumber[i]) && tableNumber[i - 2] == 0)
                {
                    tableNumber[i] = -tableNumber[i - 3];
                    tableNumber[i - 1] = -tableNumber[i - 4];

                    //Debug.Log(" Im in secound controll number i:" + tableNumber[i] + " counter:" + i + "  number i-1: " + tableNumber[i - 1] + " counter: " + (i-1));
                }
            }
            if(i > 2)
            {
                if (tableNumber[i - 3] == tableNumber[i - 1] && tableNumber[i - 2] == tableNumber[i])
                {
                    if (tableNumber[i - 3] == 0 && tableNumber[i - 1] == 0)
                    {
                        tableNumber[i - 2] = -tableNumber[i - 2];

                        //Debug.Log(" Im in last controll number i:" + tableNumber[i - 2] + " counter:" + (i - 2));
                    }
                    else if ((tableNumber[i - 3] == 1 && tableNumber[i - 1] == 1) || (tableNumber[i - 3] == -1 && tableNumber[i - 1] == -1))
                    {
                        tableNumber[i - 1] = -tableNumber[i - 1];

                        //Debug.Log(" Im in last controll number i:" + tableNumber[i - 1] + " counter:" + (i - 1));
                    }

                }
            }

        }

        Debug.Log(tableNumber[countArray - 1]);

        return tableNumber;
    }

    private void JoinRandomRoom()
    {
        if(!PhotonNetwork.InRoom && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    private void JoinToScene()
    {
        SceneManager.LoadScene("Game");
    }
    public void SetPlayerName()
    {
        string nickNamePlayer = "Janusz " + Random.Range(0,10000);
        PhotonNetwork.NickName = nickNamePlayer;
    }
}
