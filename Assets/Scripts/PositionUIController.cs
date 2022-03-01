using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;


public class PositionUIController : PlacingPlayerUI
{
    private string nameObject;
    [SerializeField] protected float positionY;

    void Start()
    { 
        nameObject = transform.name;

        SetElementOnPosition(nameObject,positionY);

    }

}
