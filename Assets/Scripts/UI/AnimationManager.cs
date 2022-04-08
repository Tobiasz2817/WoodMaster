using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public const string EMPTY_STATE = "EMPTY_STATE";
    public const string SEARCH_DOT_ANIM = "SEARCH_DOT_ANIM";
    public const string MOVE_SEARCH_UP = "MOVE_SEARCH_UP";
    public const string MOVE_SEARCH_DOWN = "MOVE_SEARCH_DOWN";
    public const string FIND_GAME = "FIND_GAME";

    public void PlayAnimation(string nameAnimation)
    {
        Animator animator;

        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
        animator.Play(nameAnimation);
    }
    public void PlayAnimation(Animator anim, string nameAnimation)
    {
        anim.Play(nameAnimation);
    }
    public void PlayAnimation(Animator anim, string nameAnimation, float timeToPlay)
    {
        StartCoroutine(WaitToPlay(anim, nameAnimation, timeToPlay)); 
    }
    IEnumerator WaitToPlay(Animator anim, string nameAnimtion ,float time)
    {
        yield return new WaitForSeconds(time);
        PlayAnimation(anim,nameAnimtion);
        
        StopAllCoroutines();
    }
}
