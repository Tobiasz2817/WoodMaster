using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MapSelection : MonoBehaviour
{
    [SerializeField] private List<Sprite> mapSprites = new List<Sprite>();
    [SerializeField] string keyProperties;


    private Image mapImage;
    private int indexer = 0;

    void Start()
    {
        mapImage = GetComponent<Image>();

        mapImage.sprite = mapSprites[indexer];


        SetMapPropertiesToPlayer();
    }

    public void RightArrowSwitch()
    {
        indexer++;

        if(indexer >= mapSprites.Count)
        {
            indexer = 0;
        }

        mapImage.sprite = mapSprites[indexer];

        SetMapPropertiesToPlayer();
    }
    public void LeftArrowSwitch()
    {
        indexer--;

        if(indexer < 0)
        {
            indexer = mapSprites.Count - 1;
        }

        mapImage.sprite = mapSprites[indexer];

        SetMapPropertiesToPlayer();    
    }
    public Sprite SpriteSelector(int index)
    {
        return mapSprites[index];
    }
    private void SetMapPropertiesToPlayer()
    {
        ExitGames.Client.Photon.Hashtable mapSelectorHash = new ExitGames.Client.Photon.Hashtable() { { keyProperties, indexer } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(mapSelectorHash);
    }
    
}
