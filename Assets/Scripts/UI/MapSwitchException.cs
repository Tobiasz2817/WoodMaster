using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MapSwitchException : MonoBehaviour
{

    [SerializeField] string wordToChange;

    public void ExceptionMessageButton()
    {
        object isReady;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(TIMBER_MULTIPLAYER.PLAYER_IS_READY, out isReady))
        {
            if ((bool)isReady)
            {
                gameObject.GetComponent<Text>().text = " Nie mo¿na zmieniæ " + wordToChange + " gdy jesteœ gotowy ";

                StartCoroutine(EnableExceptionMessage());
            }
        }
    }
    public IEnumerator EnableExceptionMessage()
    {
        yield return new WaitForSeconds(7f);
        gameObject.GetComponent<Text>().text = "";
    }
}
