using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ManageScene : MonoBehaviour
{
    public ManageScene ManageSceneHandle { set; get; }

    public static int manageScene = 0;

    private void Awake()
    {
        if(manageScene == 0)
            DontDestroyOnLoad(transform.gameObject);
        if(ManageSceneHandle == null)
            ManageSceneHandle = this;
    }

    public static int GetIndexStartedMenu()
    {
        return manageScene;
    }
    public static void SetIndexStartedMenu(int newIndex)
    {
        manageScene = newIndex;
    }
}
