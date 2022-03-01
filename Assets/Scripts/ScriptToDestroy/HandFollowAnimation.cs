using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class HandFollowAnimation : MonoBehaviourPun
{

    private InputAction inputAction;
    
    private bool isHit = false;
    private int posIndex = 0;
    private int length;
    private float t = 0;
    private string currentKey = "a";

    [SerializeField] [Range(2f,20f)] float lerpTime;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform[] positions;
    [SerializeField] Animator animator;

    

    void Start()
    {
        inputAction = playerInput.actions.actionMaps[0].actions[0];
        length = positions.Length;

        transform.position = positions[0].position;

        
    }

    void Update()
    {


        if(playerInput == null || !photonView.IsMine)
            return;

        if (inputAction.triggered)
        {
            isHit = true;
            currentKey = inputAction.activeControl.name;

            if(currentKey == "a")
            {
                posIndex = 0;
            }
            else if(currentKey == "d")
            {
                posIndex = length - 1;
            }

            animator.SetTrigger("HitTrigger");
        }

      

        if(!isHit)
        {
            return;
        }

        transform.position = Vector2.Lerp(transform.position,positions[posIndex].position, lerpTime * Time.deltaTime);

        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);

        if (t > .9f)
        {

            if (currentKey == "a")
            {

                t = 0f;

                posIndex++;

                if (posIndex >= length)
                {
                    posIndex = 0;
                    isHit = false;
                }

            }
            else if (currentKey == "d")
            {
                t = 0f;
                posIndex--;

                if (posIndex < 0)
                {
                    posIndex = length - 1;
                    isHit = false;
                }
            }

        }
    }
}
