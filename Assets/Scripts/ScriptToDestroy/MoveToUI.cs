using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveToUI : MonoBehaviour
{
    private RectTransform myTransform;
    private Button myButton;

    private bool isMove = false;
    private float newY;
    private float newX;

    [SerializeField] float time;
    [SerializeField] float timeToDisableMove;



    



    void Start()
    {
        myTransform = GetComponent<RectTransform>();
    }
    public void ChangePosElementsUI(InputAction.CallbackContext press)
    {
        if(press.action.triggered)
        {

            myTransform.anchoredPosition = new Vector2(newX, newY);

            Debug.Log(myTransform.name + " Its my RectTransform");
            Debug.Log(myTransform.anchoredPosition + " Its my anchoredPos Rect");
            Debug.Log(myTransform.position + " Its my Pos Rect");
            Debug.Log(myTransform.anchorMin + " Its anchor min");
            Debug.Log(myTransform.anchorMax  + " Its anchor max");
        }
    }
    void Update()
    {
        if(isMove == true)
        {
           

            myTransform.anchoredPosition = Vector2.Lerp(myTransform.anchoredPosition, new Vector2(newX,newY), time * Time.deltaTime);

            if(timeToDisableMove > 4f)
            {
                if(myButton != null)
                    myButton.enabled = true;
                isMove = false;
                timeToDisableMove = 0f;
            }

            timeToDisableMove += time * Time.deltaTime;

            Debug.Log(timeToDisableMove);
        }
    }
    public void SetNewX(int newX)
    {
        this.newX = newX;
    }
    public void SetNewY(int newY)
    {
        this.newY = newY;
    }
    public void IsTimeToMove(Button buttonRef)
    {

        myButton = buttonRef;

        myButton.enabled = false;
        

        isMove = true;
    }

}
