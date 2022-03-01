using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControlSpawnPosition : MonoBehaviourPun
{
    Transform[] childPosition;

    void Awake()
    {
        GameManager gameManager =  GameManager.instance;

        childPosition = gameManager.positionToSpawn;

        
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Vector2 pos = gameManager.treePositionVec2[i];
            float xPos = -1.15f;
            childPosition[i].position = pos + new Vector2(xPos, childPosition[i].position.y);
            
        }
    }    
}
