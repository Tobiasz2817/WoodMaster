using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeButtons : MonoBehaviour
{
   
    void Start()
    {
        this.GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.1f;
    }


}
