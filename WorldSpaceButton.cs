using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class WorldSpaceButton : MonoBehaviour
{
    bool playerWithinButton;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWithinButton)
        {
            GetComponent<Button>().Select();
        }
        else if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            //Debug.Log("left button but still selected");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerWithinButton = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            playerWithinButton = false;
        }
    }
}
