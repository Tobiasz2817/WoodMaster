using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Status Connecting")]
    [SerializeField] Text statusConnecting;

    [Header("Start Panel")]
    [SerializeField] GameObject startScreenPanel;
    [SerializeField] InputField nicknameInputfield;
    [SerializeField] GameObject errorMessageGameObject;
    [SerializeField] int countTable;

    [Header("Main Panel")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject searchTimeGameObject;
    [SerializeField] GameObject findGamePanel;
    [SerializeField] GameObject timeToJoin;
    [SerializeField] Button searchGameButton;
    [SerializeField] Button playSoloButton;
    [SerializeField] Button championSelectionButton;
    [SerializeField] Button settingButton;
    [SerializeField] List<Toggle> distanceTreeToggle = new List<Toggle>();

    

    [Header("Create Room Panel")]
    [SerializeField] GameObject createRoomPanel;

    [Header("Waiting Room Panel")]
    [SerializeField] GameObject waitingRoomPanel;  
    [SerializeField] GameObject playerGameObject;  
    [SerializeField] Transform playerPanelTransform;
    [SerializeField] Text playersReadyText;
    [SerializeField] GameObject readyIconGO;
    [SerializeField] GameObject startGameRoomButton;
    [SerializeField] Button leftButtonMapSelection;
    [SerializeField] Button rightButtonMapSelection;

    [Header("LoadingPanel")]
    [SerializeField] GameObject loadingPanel;

    [Header("Animators")]
    [SerializeField] GameObject animatiorManager;
    [SerializeField] Animator searchContentAnim;

    private Dictionary<int, GameObject> players;
    private bool isReady = false;
    private bool isWaiting = false;

    private int currentScene = 0;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().onEndTimerEvent += ChangeState;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.LocalPlayer.NickName = " Player " + Random.Range(1,1000000000);

           
        }

        if(ManageScene.GetIndexStartedMenu() == 0)
            ChangeStatePanel(startScreenPanel.name);
        else
            ChangeStatePanel(mainMenuPanel.name);
    }
    private IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(1f);
        isWaiting = false;
    }
    #region Callbacks Content

    public override void OnConnected()
    { 
        /*Debug.Log(" Po³¹czy³es siê z sieci¹ ");*/
    }
    public override void OnConnectedToMaster()
    {
        /*Debug.Log(" Po³¹czy³es siê z serwerem ");*/
            
        /*SetPersistentListenerStateButton(UnityEngine.Events.UnityEventCallState.RuntimeOnly);*/
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRandomRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        /*Debug.Log(" Other Player Actor number when left room " + (otherPlayer.ActorNumber));

        Debug.Log(otherPlayer.ActorNumber);*/

        Destroy(players[otherPlayer.ActorNumber].gameObject);

        players.Remove(otherPlayer.ActorNumber);


        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !waitingRoomPanel.activeInHierarchy)
        {
            findGamePanel.SetActive(false);
            timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().ResetTime();
            animatiorManager.GetComponent<AnimationManager>().PlayAnimation(searchContentAnim, AnimationManager.MOVE_SEARCH_DOWN);
        

            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && waitingRoomPanel.activeInHierarchy)
        {

            ExitRoomOrLobby();

            ChangeStatePanel(mainMenuPanel.name);
        }

        RefillingTheLooby();
    }
    public override void OnLeftRoom()
    {
        timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().ResetTime();
        findGamePanel.SetActive(false);

        foreach (var leavePlayer in players.Values)
        {
            Destroy(leavePlayer);
        }

        players.Clear();
        players = null;
        StartCoroutine(WaitForTime());
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        /*Debug.Log(newPlayer.ActorNumber - 1);*/
        
        GameObject playerGO = Instantiate(playerGameObject, playerPanelTransform);
        playerGO.transform.localScale = Vector3.one;
        playerGO.GetComponent<PlayerInitialize>().InitializePlayer(newPlayer.ActorNumber, newPlayer.NickName);

        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            playerGO.transform.GetChild(2).gameObject.SetActive(true);
            playerGO.transform.GetChild(3).gameObject.SetActive(true);
        }

        players.Add(newPlayer.ActorNumber, playerGO);

        RefillingTheLooby();


        searchTimeGameObject.GetComponent<CountDownTime>().ResetTime();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            SearchAnimMoveUp();
        }
        

        /*
        Debug.Log("Do³¹czy³em do pokoju " + PhotonNetwork.CurrentRoom.Name + " Jest w nim aktualnie " + PhotonNetwork.CurrentRoom.PlayerCount);
        */
        
    }

    private IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(1f);
        newScene(1);
    }
    public override void OnJoinedRoom()
    {
        if(!PhotonNetwork.CurrentRoom.IsVisible)
        {
            StartCoroutine(DelayTime());
            

            return;
        }

        /*SetPersistentListenerStateButton(UnityEngine.Events.UnityEventCallState.Off);*/

        /*Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber - 1);*/

        if (players == null)
        {
            players = new Dictionary<int, GameObject>();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            
            RefillingTheLooby();
           

            searchTimeGameObject.GetComponent<CountDownTime>().ResetTime();

            SearchAnimMoveUp();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerGO = Instantiate(playerGameObject, playerPanelTransform);
            playerGO.transform.localScale = Vector3.one;
            playerGO.GetComponent<PlayerInitialize>().InitializePlayer(player.ActorNumber, player.NickName);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber)
            {
                playerGO.transform.GetChild(2).gameObject.SetActive(true);
                playerGO.transform.GetChild(3).gameObject.SetActive(true);
            }

            players.Add(player.ActorNumber, playerGO);
        }

        /*
        Debug.Log("Do³¹czy³em do pokoju " + PhotonNetwork.CurrentRoom.Name + " Jest w nim aktualnie " + PhotonNetwork.CurrentRoom.PlayerCount);
    */
    }
    public override void OnCreatedRoom()
    {
        /*
        Debug.Log("Utworzy³em pokój " + PhotonNetwork.CurrentRoom.Name);   
    */
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        if(changedProps.ContainsKey(TIMBER_MULTIPLAYER.PLAYER_IS_READY))
        {
            List<int> mapSelectionList = new List<int>();
            int sumReadyPlayer = 0;

            foreach (var player in PhotonNetwork.PlayerList)
            {

                object isReady;
                if (player.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_IS_READY, out isReady))
                {
                    if ((bool)isReady)
                    {
                        sumReadyPlayer++;


                        object mapIndex;
                        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_MAP_SELECTION, out mapIndex))
                        {
                            mapSelectionList.Add((int)mapIndex);
                        }
                    }


                    if(sumReadyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            startGameRoomButton.SetActive(true);
                        }
                        else
                        {
                            startGameRoomButton.SetActive(false);
                        }

                        // Logika w której bêdzie dana mapa dodana do pokoju
                        
                        if(PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            Dictionary<int,int> counterTable = new Dictionary<int,int>();  
                            int finallyIndexerMap = 0;

                            foreach (var indexMap in mapSelectionList)
                            {
                                if(counterTable.ContainsKey(indexMap))
                                {
                                    counterTable[indexMap]++;
                                }
                                else
                                {
                                    counterTable[indexMap] = 1;
                                }
                        
                            }

                            List<int> theBiggestValue = new List<int>();
                            List<int> theLowestValue = new List<int>();
                            foreach (var indexMap in counterTable)
                            {
                                if(indexMap.Value > 1)
                                {
                                    theBiggestValue.Add(indexMap.Key);
                                }
                                else
                                {
                                    theLowestValue.Add(indexMap.Key);
                                }
                            }

                            if (theBiggestValue.Count == 0)
                                finallyIndexerMap = theLowestValue[Random.Range(0,theLowestValue.Count)];                        
                            else if(theBiggestValue.Count == 1)
                                finallyIndexerMap = theBiggestValue[0]; 
                           
                            else if(theBiggestValue.Count == 2)
                            {
                                if(theBiggestValue[0] < theBiggestValue[1])
                                    finallyIndexerMap = theBiggestValue[1];                                 
                                else
                                    finallyIndexerMap = theBiggestValue[0];
                            }


                            ExitGames.Client.Photon.Hashtable mapSelectorProperties = new ExitGames.Client.Photon.Hashtable();
                            mapSelectorProperties.Add(TIMBER_MULTIPLAYER.MAP_SELECTION, finallyIndexerMap);
                                
                            PhotonNetwork.CurrentRoom.SetCustomProperties(mapSelectorProperties);
                        }
                    }
                    else
                    {
                        startGameRoomButton.SetActive(false);
                    }
                }

            }
            playersReadyText.text = "Players ready " + sumReadyPlayer;
        }
        else if(changedProps.ContainsKey(TIMBER_MULTIPLAYER.PLAYER_HEROS_SELECTION))
        { 
            object indexSprite;
            if(targetPlayer.CustomProperties.TryGetValue((TIMBER_MULTIPLAYER.PLAYER_HEROS_SELECTION), out indexSprite))
            {
                GameObject player = players[targetPlayer.ActorNumber];
                player.transform.GetChild(0).GetComponent<Image>().sprite = player.transform.GetChild(0).GetComponent<MapSelection>().SpriteSelector((int)indexSprite);
            }

        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameRoomButton.SetActive(true);
        }
        else
        {
            startGameRoomButton.SetActive(false);
        }
    }
    #endregion


    #region Public UI Methods
    public void StartGameButton()
    {
        newScene(1);
    }    
    public void ChangeStateReadyButton()
    {
        isReady = !isReady;

        SetPersistentListener(leftButtonMapSelection,rightButtonMapSelection);

        Button leftPlayerChampionSelect = players[PhotonNetwork.LocalPlayer.ActorNumber].transform.GetChild(2).GetComponent<Button>();
        Button rightPlayerChampionSelect = players[PhotonNetwork.LocalPlayer.ActorNumber].transform.GetChild(3).GetComponent<Button>();
        SetPersistentListener(leftPlayerChampionSelect,rightPlayerChampionSelect);    
        
        ExitGames.Client.Photon.Hashtable isReadyProperties = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_IS_READY, isReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(isReadyProperties);

        readyIconGO.SetActive(isReady);
    }
    public void ConfirmButton()
    {
        string nickname = nicknameInputfield.text;

        if (string.IsNullOrEmpty(nickname) || nickname.Length < 4)
        {
            errorMessageGameObject.SetActive(true);

            if (string.IsNullOrEmpty(nickname)) ErrorMessage("Place holder can't be empty \n please set your nickname");
            else ErrorMessage("Nick name must be longest than 3 numbers");

            return;
        }

        PhotonNetwork.LocalPlayer.NickName = nickname;

        ChangeStatePanel(mainMenuPanel.name);
    }
    
    public void StartSearchMatchmaking()
    {
        if (isWaiting == true) return;

        animatiorManager.GetComponent<AnimationManager>().PlayAnimation(searchContentAnim,AnimationManager.MOVE_SEARCH_DOWN);

        ExitGames.Client.Photon.Hashtable dictionaryEntries = new ExitGames.Client.Photon.Hashtable();

        int isOnCount = 0;
        foreach (var toggle in distanceTreeToggle)
        {
            if (toggle.isOn)
            {
                string toggleKey = toggle.transform.GetChild(1).GetComponent<Text>().text;
                dictionaryEntries.Add(toggleKey, int.Parse(toggleKey));
         
                isOnCount++;
            }
        }

        foreach (var item in dictionaryEntries.Keys)
        {
            /*
            Debug.Log("START SEARCH MATCHMAKING KEY " + item);
        */
        }

        if (isOnCount == distanceTreeToggle.Count - 1 || isOnCount == 0)
        {
            PhotonNetwork.JoinRandomRoom();
            return;
        }
 
        PhotonNetwork.JoinRandomRoom(dictionaryEntries,0);
        
        isWaiting = true;
    }
    public void LeaveReffilingRoom()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    public void LeftRoomButton()
    {
        ChangeStatePanel(mainMenuPanel.name);

        ExitRoomOrLobby();
    }

    public void StopSearchMatchmaking()
    {
        animatiorManager.GetComponent<AnimationManager>().PlayAnimation(searchContentAnim, AnimationManager.MOVE_SEARCH_UP);
        ExitRoomOrLobby();
    }
    public void SoloRoomEntry()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InLobby || PhotonNetwork.InRoom) return;
        if (isWaiting) return;
        
        ChangeStatePanel(loadingPanel.name);

        string roomName = "Room " + Random.Range(1,10000000);
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 1;
        roomOptions.IsVisible = false;
        roomOptions.IsOpen = false;

        string[] randomBranch = { TIMBER_MULTIPLAYER.RANDOM_NUMBER_TABLE };
        ExitGames.Client.Photon.Hashtable custonPropertiesGame = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.RANDOM_NUMBER_TABLE, RandomBranchPos(SetDistanceTreeByToggle()) } };
        roomOptions.CustomRoomPropertiesForLobby = randomBranch;
        roomOptions.CustomRoomProperties = custonPropertiesGame;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region Private Methods
    private void CreateRandomRoom()
    {
        string roomName = "Room " + Random.Range(1, 100000000);

        RoomOptions options = new RoomOptions();
        options.IsVisible = true;
        options.MaxPlayers = 4;
        options.IsOpen = true;


        int valueProperties = SetDistanceTreeByToggle();
        string keyRoom = valueProperties.ToString();
        string[] randomBranch = { keyRoom };

        /*Debug.Log(" VALUE PROPERTIES " + valueProperties);
        Debug.Log(" KEY PROPERTIES IN CREATE ROOM " + keyRoom);*/

        ExitGames.Client.Photon.Hashtable custonPropertiesGame = new ExitGames.Client.Photon.Hashtable();
        custonPropertiesGame.Add(keyRoom, valueProperties);
        custonPropertiesGame.Add(TIMBER_MULTIPLAYER.RANDOM_NUMBER_TABLE, RandomBranchPos(valueProperties));
        options.CustomRoomPropertiesForLobby = randomBranch;
        options.CustomRoomProperties = custonPropertiesGame;

        PhotonNetwork.CreateRoom(roomName, options);
        
        
        isWaiting = true;
    }
    private int SetDistanceTreeByToggle()
    {
        List<int> confirmLengthTree = new List<int>();

        foreach (var toggle in distanceTreeToggle)
        {
            if(toggle.isOn)
            {
                int parseToInt;
                if(int.TryParse(toggle.transform.GetChild(1).GetComponent<Text>().text, out parseToInt))
                {
                    confirmLengthTree.Add(parseToInt);
                }
            }
        }
        int getRandomIndex = 0;
        if (confirmLengthTree.Count > 0)
            getRandomIndex = confirmLengthTree[Random.Range(0, confirmLengthTree.Count)];
        else if (confirmLengthTree.Count == 0)
            return countTable;


        return getRandomIndex;
    }
    private void ExitRoomOrLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
    }
    private void SetPersistentListener(Button leftButton, Button rightButton)
    {
        if(isReady)
        {
            leftButton.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.Off);
            rightButton.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.Off);
        }
        else
        {
            if (leftButton.onClick.GetPersistentListenerState(1) == UnityEngine.Events.UnityEventCallState.Off)
            {
                leftButton.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
            }
            if (rightButton.onClick.GetPersistentListenerState(1) == UnityEngine.Events.UnityEventCallState.Off)
            {
                rightButton.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
            }
        }
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

            while (true)
            {
                tableNumber[i] = Random.Range(-1, 2);

                if (tableNumber[i] != tableNumber[i - 1])
                {
                    if (tableNumber[i] == 0 || tableNumber[i - 1] == 0)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }

                }
                else if (tableNumber[i] == tableNumber[i - 1])
                {
                    if (tableNumber[i] == 0 && tableNumber[i - 1] == 0)
                    {
                        continue;
                    }
                    else if (tableNumber[i] == tableNumber[i - 1] && tableNumber[i] != tableNumber[i - 2])
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
            if (i > 2)
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

        /*Debug.Log(tableNumber[countArray - 1]);*/

        return tableNumber;
    }
    private void SearchAnimMoveUp()
    {
        AnimationManager animationManager = animatiorManager.GetComponent<AnimationManager>();
        animationManager.PlayAnimation(searchContentAnim, AnimationManager.FIND_GAME);
        animationManager.PlayAnimation(searchContentAnim, AnimationManager.MOVE_SEARCH_UP, 2f);
    }
    private void RefillingTheLooby()
    {
        findGamePanel.SetActive(true);
        timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().ResetTime();

        if(PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().InvokeEventInTime(0.05f,0f);
        }
        else
        {
            timeToJoin.transform.GetChild(1).GetComponent<CountDownTime>().StartCountDownTimer();
        }
        findGamePanel.transform.GetChild(1).GetComponent<Text>().text = "Liczba graczy: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

    }
    private void ErrorMessage(string errorMessage)
    {
        errorMessageGameObject.GetComponent<Text>().text = errorMessage;
    }
    private void ChangeStatePanel(string namePanel)
    {
        startScreenPanel.SetActive(namePanel.Equals(startScreenPanel.name));
        mainMenuPanel.SetActive(namePanel.Equals(mainMenuPanel.name));
        createRoomPanel.SetActive(namePanel.Equals(createRoomPanel.name));
        waitingRoomPanel.SetActive(namePanel.Equals(waitingRoomPanel.name));
        loadingPanel.SetActive(namePanel.Equals(loadingPanel.name));
    }
    private void ChangeState()
    {
        ChangeStatePanel(waitingRoomPanel.name);

        PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }
    public void newScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion



}
