using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class diageticButton : MonoBehaviour
{
    [SerializeField] AudioClip pressSound;
    [SerializeField] Vector3 raycastOffset;
    [SerializeField] float raycastDistance = 1;
    [SerializeField] LayerMask triggerLayers;
    [Header("Collider")]
    [SerializeField] Vector2 pressedHitboxOffset;
    [SerializeField] Vector2 pressedHitboxSize;
    Vector2 unPressedHitboxSize;
    Vector2 unPressedHitboxOffset;


    [SerializeField] bool onlyPressOnce = true;
    bool pressed;

    [SerializeField] Animator ani;

    public UnityEvent OnPressed;
    // Start is called before the first frame update
    void Start()
    {
        unPressedHitboxSize = GetComponent<BoxCollider2D>().bounds.size;
        unPressedHitboxOffset = GetComponent<BoxCollider2D>().offset;
    }

    // Update is called once per frame
    void Update()
    {
        bool buttonBeingPressed = PlayerCastCheck();
        if(buttonBeingPressed && !pressed)
        {
            Press();
        }

        if(!buttonBeingPressed && pressed && !onlyPressOnce)
        {
            UnPress();
        }
    }

    bool PlayerCastCheck()
    {
        Vector3 rayPosition = transform.position + raycastOffset;

        bool upHit = Physics2D.Raycast(rayPosition, transform.up, raycastDistance, triggerLayers);

        Debug.DrawRay(rayPosition, Vector3.up * raycastDistance, Color.red);


        return upHit;
    }

    void Press()
    {
        if(ani != null)
        {
            ani.SetBool("pressed",true);
        }

        if(pressSound != null)
        {
            AudioManager.PlaySound(pressSound.name,1f,0.4f);
        }

        Debug.Log("pressed !");

        pressed = true;

        Invoke("SetHitbox", 0.2f);

        OnPressed.Invoke();
    }

    void UnPress()
    {
        if (ani != null)
        {
            ani.SetBool("pressed", true);
        }

        pressed = false;
        SetHitbox();
    }

    void SetHitbox()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        collider.size = pressed ? pressedHitboxSize : unPressedHitboxSize;

        collider.offset = pressed ? pressedHitboxOffset : unPressedHitboxOffset;
    }
}
