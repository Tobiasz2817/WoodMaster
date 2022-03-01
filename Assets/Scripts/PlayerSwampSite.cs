using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Smooth;

public class PlayerSwampSite : MonoBehaviourPun
{
    private string currentKey = "a";
    private GameManager gameManager;
    
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] SpriteRenderer rightHandSprite;
    [SerializeField] GameObject gun;
    SmoothSyncPUN2 sync;

    void Start()
    {
        gameManager = GameManager.instance;
        sync = GetComponent<SmoothSyncPUN2>();
    }

    
    void Update()
    {
        
    }
    public void SwampPos(InputControl inputControl,Vector2 leftSite,Vector2 rightSite)
    {
        if (currentKey == inputControl.name)
        {
           return;
        }

        photonView.RPC("SyncSwamp", RpcTarget.AllBuffered, leftSite, rightSite, inputControl.name);

        currentKey = inputControl.name;
        Debug.Log(" Methods iterrations" ); 
    }

    [PunRPC]
    public void SyncSwamp(Vector2 leftSite,Vector2 rightSite, string currentKey)
    {
         Debug.Log("left site " + leftSite + " right Site " + rightSite);

        if (currentKey == "a")
        {
            sync.teleportAnyObject(leftSite,Quaternion.identity, transform.localScale);
            gun.transform.localPosition = new Vector2(-0.005f, 0);
        }
        else if (currentKey == "d")
        {
            sync.teleportAnyObject(rightSite, Quaternion.identity, transform.localScale);
            gun.transform.localPosition = new Vector2(-0.335f, 0);
        }

        playerSprite.flipX = !playerSprite.flipX;
        rightHandSprite.flipX = !rightHandSprite.flipX;
        gun.GetComponent<SpriteRenderer>().flipX = !gun.GetComponent<SpriteRenderer>().flipX;

        Debug.Log("Flip now");
    }
}
