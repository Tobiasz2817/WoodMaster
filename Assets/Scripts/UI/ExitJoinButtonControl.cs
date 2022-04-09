using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExitJoinButtonControl : MonoBehaviour
{
    private float anchorUp = 787.8f;
    private Button button;

    [SerializeField] private RectTransform rect;
    void Start()
    {
        button = GetComponent<Button>();
        Debug.Log(rect.anchoredPosition3D.y);
    }
    void Update()
    {
        if (rect != null)
        {
            if (rect.anchoredPosition3D.y < anchorUp - 1)
            {
                if(button.onClick.GetPersistentListenerState(0) == UnityEventCallState.RuntimeOnly)
                    button.onClick.SetPersistentListenerState(0,UnityEventCallState.Off);
            }
            else
            {
                if (button.onClick.GetPersistentListenerState(0) == UnityEventCallState.Off) 
                    button.onClick.SetPersistentListenerState(0,UnityEventCallState.RuntimeOnly);
                
                Debug.Log(button.onClick.GetPersistentListenerState(0) == UnityEventCallState.Off);
            }
        }
    }
}
