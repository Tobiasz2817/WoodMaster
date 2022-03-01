using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPun
{
    GameManager gameManager;
    PlayerSwampSite playerSwampSite;
    PlayerInput playerInput;
    

    [SerializeField] List<GameObject> treeList = new List<GameObject>();

    GameObject branchPrefab;
    GameObject treePrefab;

    [SerializeField] private int playerID;
    public bool playerGameOver = false;

    [SerializeField] float freezeTime;

    [SerializeField] int[] randomRangeTable;

    [SerializeField] GameObject endGameUI;

    bool test = false;
    private void Awake()
    {
        endGameUI.SetActive(false);

        // Initialize 
        gameManager = GameManager.instance;
        this.treePrefab = gameManager.treePrefab;
        this.branchPrefab = gameManager.branchPrefab;



        // Pos tree / respawn Tree 
        playerID = gameManager.SetCorectlyPlayerID();

        // Create number for branch
        randomRangeTable = new int[gameManager.countTree];
    }
    
    void Start()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
            return;

        if (photonView.IsMine)
        {
            object value;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.RANDOM_NUMBER_TABLE, out value))
            {
                int[] tableNumberTree = (int [])value;
                photonView.RPC("CloneTable",RpcTarget.AllBuffered, tableNumberTree);
                gameManager.countTree = tableNumberTree.Length;

                Debug.Log(randomRangeTable.Length);
            }
        }

        // Player Input (Add event itp.)
        playerInput = GetComponent<PlayerInput>();
        playerSwampSite = GetComponent<PlayerSwampSite>();  
        playerInput.actionEvents[0].AddListener(HittingTree);

        
        if(photonView.IsMine)
        {
            photonView.RPC("CreateTree", RpcTarget.AllBuffered, gameManager.treePositionVec2[playerID - 1].x, playerID);
        }


    }
    void Update()
    {
        if(treeList.Count != 0 || !gameManager.isGameStart || playerGameOver || !photonView.IsMine)
            return;
       

        playerGameOver = true;
        string myNickName = PhotonNetwork.LocalPlayer.NickName;
        IncreasePlacePlayer(myNickName);

        ExitGames.Client.Photon.Hashtable isStillLife = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_DIE, playerGameOver } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(isStillLife);

        StartCoroutine(CheckPlayerDie());

    }
    [PunRPC]
    private void CloneTable(int[] branchPosition)
    {
        randomRangeTable = branchPosition;
    }
    [PunRPC]
    public void CreateTree(float spawnOnX, int playerID) 
    {
        Debug.Log("Func 'Create Tree' playerID = " + playerID);

        for (int i = 0; i < gameManager.countTree; i++)
        {
            GameObject tree = Instantiate(treePrefab,new Vector3(spawnOnX,i + 1,0),Quaternion.identity);
            treeList.Add(tree);

            
        }

        for (int i = 0; i < treeList.Count; i++)
        {
            Instantiate(branchPrefab, new Vector2(treeList[i].transform.position.x + randomRangeTable[i], treeList[i].transform.position.y), Quaternion.identity, treeList[i].transform);
        }

    }
    public void TestNewBinds(InputAction.CallbackContext hit)
    {
        if(hit.action.triggered)
        {
            Debug.Log("JAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }

    public void HittingTree(InputAction.CallbackContext hit)
    {
        if(!gameManager.isGameStart || playerGameOver || !photonView.IsMine)
        { 
            return;
        }

        if (hit.action.triggered)
        {

            Debug.Log("hit");

            object value;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_SWAMP_SIDE, out value))
            {
                Vector2[] list = (Vector2[])value;
                Vector2 leftSite = list[0];
                Vector2 rightSite = list[1];

                playerSwampSite.SwampPos(hit.control, leftSite, rightSite);
            }


            photonView.RPC("MultiplyScore", RpcTarget.AllBuffered, 1);


        
           StartCoroutine(FreezeHitTime());
        }


    }
    IEnumerator FreezeHitTime()
    {
        yield return new WaitForSeconds(freezeTime);
        photonView.RPC("HitTree", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void HitTree()
    {
        if(playerGameOver)
            return;

        List<GameObject> treeListObj = treeList;

        for (int j = treeListObj.Count - 1; j > 0; j--)
        {
            treeListObj[j].transform.position = treeListObj[j - 1].transform.position;
        }

        Destroy(treeListObj[0].gameObject);

        treeListObj.Remove(treeListObj[0]);

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Branch"))
        {
            if(photonView.IsMine)
            {
                playerGameOver = true;
                photonView.RPC("TurnUI", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, playerGameOver);

                ExitGames.Client.Photon.Hashtable isStillLife = new ExitGames.Client.Photon.Hashtable() { { TIMBER_MULTIPLAYER.PLAYER_DIE, playerGameOver } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(isStillLife);

                StartCoroutine(CheckPlayerDie());
            }
        }

        Debug.Log("Collision with " + collision.name);
    }
    IEnumerator CheckPlayerDie()
    {
        yield return new WaitForSeconds(0.3f);
        gameManager.CheckWhenALlPlayersDieOrWin();
    }
    [PunRPC]
    private void TurnUI(string myNickName, bool playerGameOver)
    {
        this.playerGameOver = playerGameOver;

        if (endGameUI == null)
            return;

        endGameUI.SetActive(true);
        endGameUI.GetComponent<Text>().text = "Przegra³ gracz o nicku : " + myNickName;

    }
    private void IncreasePlacePlayer(string myNickname)
    {   
        int myID = photonView.ViewID; 

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions()
        {
            Reliability = false
        };


        object[] playersParameters = new object[] { myNickname , myID };

        PhotonNetwork.RaiseEvent((byte)GameFInishedEnum.CurrentStateGame.whoWinGame, playersParameters, raiseEventOptions, sendOptions);
        
    }

    
}
