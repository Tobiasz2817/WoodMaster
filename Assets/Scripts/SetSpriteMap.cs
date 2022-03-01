using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetSpriteMap : MonoBehaviour
{
    [SerializeField] List<Sprite> spriteMapList;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        object indexMap;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.MAP_SELECTION, out indexMap))
        {
            spriteRenderer.sprite = spriteMapList[(int)indexMap];
        }
    }

}
