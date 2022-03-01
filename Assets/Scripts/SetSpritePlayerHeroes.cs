using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetSpritePlayerHeroes : MonoBehaviourPun
{
    [SerializeField] List<Sprite> heroesSprites = new List<Sprite>();
    [SerializeField] SpriteRenderer mySpritePlayer;
    void Start()
    {
        if(photonView.IsMine)
        {
            object indexSpriteToChange;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_HEROS_SELECTION, out indexSpriteToChange))
            {
                photonView.RPC("ChangeSpritePlayerHeroes", RpcTarget.AllBuffered, (int)indexSpriteToChange);
            }
        }
    }

    [PunRPC]
    private void ChangeSpritePlayerHeroes(int indexSpriteToChange)
    {
        mySpritePlayer.sprite = heroesSprites[(int)indexSpriteToChange];    
    }
}
