using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<Vector2> treePositionVec2 = new List<Vector2>();
    public GameObject[] playerPrefab;
    public Transform[] positionToSpawn;
    public GameObject branchPrefab;
    public GameObject restartLobby;
    public int[] randomRangeTable;

    public bool isGameStart = true;

    public static GameManager instance;
    
    public int numberPlace = 1;
    void Awake()
    {
        object count;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.RANDOM_NUMBER_TABLE, out count))
        {
            countTree = ((int[])count).Length;
        }
        
        if(instance != null)
        {
            instance = null;
        }

        instance = this;   
        
       
        TreePosition();
     
    }

    public GameObject treePrefab;
    public int countTree;

    // PlayerUIController
    public TextMeshProUGUI countTime;
    public GameObject playerUI;

    void Start()
    {
        

        if(PhotonNetwork.IsConnected)
        {
            int myActorNumber = SetCorectlyPlayerID() - 1;
            PhotonNetwork.Instantiate(playerPrefab[0].name, positionToSpawn[myActorNumber].position, Quaternion.identity);


            // Set for all Players Properties
            Vector2 leftSite = new Vector2( treePositionVec2[myActorNumber].x - 1.15f, positionToSpawn[myActorNumber].position.y);
            Vector2 rightSite = new Vector2(treePositionVec2[myActorNumber].x + 1.15f, positionToSpawn[myActorNumber].position.y);

            Vector2[] list = new Vector2[2];
            list[0] = leftSite;
            list[1] = rightSite;

            ExitGames.Client.Photon.Hashtable swampPosProp = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_SWAMP_SIDE, list }};
            PhotonNetwork.LocalPlayer.SetCustomProperties(swampPosProp);

            bool isDie = false;
            ExitGames.Client.Photon.Hashtable isStillLife = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_DIE, isDie } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(isStillLife);
        }
    }
    public void CheckWhenALlPlayersDieOrWin()
    {
        int x = 0;
        int playersCountInRoom = PhotonNetwork.CurrentRoom.PlayerCount;

        Player[] player = PhotonNetwork.PlayerList;
        for (int i = 0; i < playersCountInRoom; i++)
        {
            object value;
            if(player[i].CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_DIE,out value))
            {
                /*Debug.Log(" Value is Equals " + (bool)value);*/

                if ((bool)value == true)
                {
                    x++;

                    /*Debug.Log(" X is Equals = " + x);*/
                }
            }
        }

        // Output 
        // U Pierwszego gracza value jest równe false 
        // Sprawdzenie u nastêpnego gracza pokaza³o ¿e nastêpne wartoœci s¹ true
        // mo¿e sprubój na ienumerator

        if(x == playersCountInRoom)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions()
            {
                Reliability = false
            };


            object[] playersParameters = new object[] { };

            PhotonNetwork.RaiseEvent((byte)GameFInishedEnum.CurrentStateGame.gameIsOver, playersParameters, raiseEventOptions, sendOptions);
        }


        /*Debug.Log(" Methods CheckWhenALlPlayersDieOrWin " + x + playersCountInRoom);*/
    }

    public IEnumerator SetActiveRestartLobby()
    {
        yield return new WaitForSeconds(3f);
        LoadMainMenuExitButton();
    }
    public void LoadMainMenuExitButton()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        ManageScene.SetIndexStartedMenu(1);
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    public void TreePosition()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float widthMin = -halfWidth;
        float widthMax = halfWidth;

        float width = -widthMin + widthMax;
        float lengthOneX = width / (playerCount + 1);

        // from -infi to 0
        float x1 = widthMin + lengthOneX;
        float x2 = x1 + lengthOneX;

        //from 0 to infi
        float x3 = x2 + lengthOneX;
        float x4 = x3 + lengthOneX;

        float[] table = {x1,x2,x3,x4};

        for (int i = 0; i < playerCount; i++)
        {
            treePositionVec2.Add(new Vector2(table[i],0));
        }
             
    }
    public int SetCorectlyPlayerID()
    {

        Dictionary<int, int> actorNumbersDictionary = new Dictionary<int, int>();

        List<int> actorNumbers = new List<int>();

        int myNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            actorNumbers.Add(player.ActorNumber);

        }
        actorNumbers.Sort();

        int i = 1;
        foreach (var actorNumber in actorNumbers)
        {
            actorNumbersDictionary.Add(actorNumber, i);

            /*Debug.Log(" Number " + actorNumber + " indexItter " + i);*/
            i++;
        }


        int indexMyNumber = 0;
        if (actorNumbersDictionary.ContainsKey(myNumber))
        {
            indexMyNumber = actorNumbersDictionary[myNumber];
        }


        /*Debug.Log(" Index my Number: " + indexMyNumber);*/

        return indexMyNumber;
    }

}
