using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class ControlPermisionButtons : MonoBehaviour
{
    [SerializeField]
    private List<Button> controledButtons;

    [SerializeField] private Button searchCancelButton;
    [SerializeField] private GameObject findRoomGo;
    private Coroutine coroutineX;

    private Coroutine coroutine;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom || PhotonNetwork.InLobby)
        {
            ChangeButtonPersistent(UnityEventCallState.Off);
        }
        else if(PhotonNetwork.IsConnected)
        {
            if(coroutine == null)
                coroutine = StartCoroutine(TimeForEnableButtons(1f));
        }

        if (findRoomGo.activeInHierarchy)
        {
            if(searchCancelButton.onClick.GetPersistentListenerState(1) == UnityEventCallState.RuntimeOnly)
                searchCancelButton.onClick.SetPersistentListenerState(1,UnityEventCallState.Off);
        }
        else
        {
            if (coroutineX == null)
                coroutineX = StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        if (searchCancelButton.onClick.GetPersistentListenerState(1) == UnityEventCallState.Off) 
            searchCancelButton.onClick.SetPersistentListenerState(1,UnityEventCallState.RuntimeOnly);

        coroutineX = null;
    }
    public void ButtonControl()
    {
        ChangeButtonPersistent(UnityEventCallState.Off);

        StartCoroutine(TimeForEnableButtons(1f));
    }
    private void ChangeButtonPersistent(UnityEventCallState callState)
    {
        foreach (var button in controledButtons)
        {
            int length = button.onClick.GetPersistentEventCount();
            for (int i = 0; i < length; i++)
            {
                if (button.onClick.GetPersistentListenerState(i) == callState) return;
                
                button.onClick.SetPersistentListenerState(i,callState);
            }

        }
    }
    private IEnumerator TimeForEnableButtons(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeButtonPersistent(UnityEventCallState.RuntimeOnly);

        coroutine = null;
    }
}
