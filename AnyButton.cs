using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnyButton : MonoBehaviour
{
    [SerializeField] UnityEvent onAnyButton;
    // Update is called once per frame
    void Update()
    {
        if(AnyInput())
        {
            onAnyButton.Invoke();
        }
    }

    public static bool AnyInput()
    {
        return Input.anyKeyDown || Input.GetButtonDown("Interact") || Input.GetButtonDown("Primary")|| Input.GetButtonDown("Secondary") || Input.GetButtonDown("Jump");
    }
}
