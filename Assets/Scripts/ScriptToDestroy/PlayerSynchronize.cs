using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Smooth;
using UnityEngine.InputSystem;


public class PlayerSynchronize : MonoBehaviourPun
{
    [SerializeField] SmoothSyncPUN2 sync;
    [SerializeField] float timeToSync;

    [SerializeField] Vector3 positionTestRight;
    [SerializeField] Vector3 positionTestLeft;
    [SerializeField] Quaternion rotationTest;
    private void Awake()
    {
        sync = GetComponent<SmoothSyncPUN2>();
    }

    void Update()
    {

        if(!photonView.IsMine)
        {
            return;
        }    
      
        
        
        Debug.Log("Position player: " + PhotonNetwork.LocalPlayer.NickName  + " " + sync.getPosition());

       
    }


    public void TestTrigger(InputAction.CallbackContext press)
    {

        
        if(press.action.triggered)
        {
            if(photonView.IsMine)
            {

                Debug.Log("I press : " + press.control.name);

                if(press.control.name == "t")
                {
                    sync.teleportAnyObject(positionTestRight, rotationTest, transform.localScale);
                }
                else if(press.control.name == "r")
                {
                    sync.teleportAnyObject(positionTestLeft, rotationTest, transform.localScale);
                }

            }
        }
    }
    

}
