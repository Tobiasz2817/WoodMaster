using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  
using UnityEngine.InputSystem;

public class PlayerPosUI : MonoBehaviourPun
{
    private GameManager gameManager;

    [SerializeField] RectTransform scoreText;
    [SerializeField] RectTransform timerPanel;
    [SerializeField] RectTransform endGameText;
    [SerializeField] RectTransform positionPlaceText;
    
    [SerializeField] GameObject playerUI;

    [SerializeField] float yPositionScoreText;
    [SerializeField] float yPositionTimerText;
    [SerializeField] float yPositionEndGameText;
    [SerializeField] float yPositionPositionPlaceText;
    
    void Start()
    {
        //gameManager = GameManager.instance;      
        
        if(playerUI.GetComponent<Canvas>().worldCamera == null)
            playerUI.GetComponent<Canvas>().worldCamera = Camera.main;

        if (photonView.IsMine)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            photonView.RPC("SyncScoreText", RpcTarget.AllBuffered, actorNumber);
        }
    }
    [PunRPC]
    public void SyncScoreText(int actorNumber)
    {

        // Master Client reload to game faster then client so we must initialize Camera and GameManager
        // because start method Master Client is more faster then client start method and Client don't have references to GM and Camera

        if(playerUI.GetComponent<Canvas>().worldCamera == null)
        {
            playerUI.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        if (gameManager == null)
        { 
            gameManager = GameManager.instance;
        }

        Vector2 treePosition = gameManager.treePositionVec2[actorNumber];

        Vector2 scorePosition = treePosition + new Vector2(0, yPositionScoreText);
        Vector2 textPosition = treePosition + new Vector2(0, yPositionTimerText);
        Vector2 endGamePosition = treePosition + new Vector2(0, yPositionEndGameText);
        Vector2 placePosition = treePosition + new Vector2(0, yPositionPositionPlaceText);

        SetOnPositionUI(scoreText, scorePosition);
        SetOnPositionUI(timerPanel, textPosition);
        SetOnPositionUI(endGameText, endGamePosition);
        SetOnPositionUI(positionPlaceText, placePosition);
    }

    private void SetOnPositionUI(RectTransform rectToConvert, Vector2 positionToConvert)
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerUI.GetComponent<RectTransform>(), Camera.main.WorldToScreenPoint(positionToConvert), Camera.main, out anchoredPos);
        rectToConvert.anchoredPosition = anchoredPos;
    }
}
