using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlacingPlayerUI : MonoBehaviourPun
{
    protected RectTransform elementRectTransform;
    protected GameObject playerUI;
    protected GameManager gameManager;

    [SerializeField] string nameUI;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }
    protected void SetElementOnPosition(string nameObject, float positionY)
    {
        if (photonView.IsMine)
        {
            //int actorNumber = GameObject.Find("GameManager").GetComponent<GameManager>().SetCorectlyPlayerID();
            int actorNumber = gameManager.SetCorectlyPlayerID() - 1;
            photonView.RPC("SyncElement", RpcTarget.AllBuffered,nameObject, actorNumber, positionY);

        }
    }

    [PunRPC]
    private void SyncElement(string nameObjectToChangePos, int actorNumber, float positionY)
    {
        playerUI = transform.Find(nameUI).gameObject;

        elementRectTransform = playerUI.transform.Find(nameObjectToChangePos).GetComponent<RectTransform>();

        if (playerUI == null)
        {
            return;
        }

        if (playerUI.GetComponent<Canvas>().worldCamera == null)
        {
            playerUI.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }

        Vector2 treePosition = gameManager.treePositionVec2[actorNumber];

        Vector2 elementPosition = treePosition + new Vector2(0, positionY);

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerUI.GetComponent<RectTransform>(), Camera.main.WorldToScreenPoint(elementPosition), Camera.main, out anchoredPos);
        elementRectTransform.anchoredPosition = anchoredPos;


    }
}
