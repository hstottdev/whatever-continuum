using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSection : MonoBehaviour
{
    public bool active;
    public MenuState state;

    public MenuSection parentSection;
    [SerializeField] Button startingButton;
    [SerializeField] float selectButtonDelay;
    [Tooltip("What should run to transition this menu onto the screen?")]
    public UnityEvent ShowEvent;
    [Tooltip("What should run to transition away this menu?")]
    public UnityEvent HideEvent;

    [Tooltip("What should run to remove this menu from the screen immediately?")]
    public UnityEvent InstaHide;

    public void Awake()
    {
        ShowEvent.AddListener(QueueButtonSelect);
    }


    void  QueueButtonSelect()
    {
        if(startingButton != null)
        {
            Invoke("SelectStartButton",selectButtonDelay);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void SelectStartButton()
    {
        InputChecker.startButton = startingButton;

        if(InputChecker.Instance.GetInputType() == InputChecker.InputType.Controller)
        {
            startingButton.Select();
        }
    }

}
