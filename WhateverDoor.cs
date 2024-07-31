using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhateverDoor : MonoBehaviour
{
    PlayerController player;
    [SerializeField] GameObject TheWhateverSeal;
    bool playerNearby;
    bool becameSeal;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool isSeal = PlayerController.instance.GetComponent<WhateverSeal>() != null;

        if (!isSeal && !becameSeal && !ButtonManager.paused)
        {
            if (player != null)
            {
                becameSeal = true;
                dimensionalObject p = player.GetComponent<dimensionalObject>();
                p.StartCoroutine(p.ScaleLerpSwap(TheWhateverSeal));//swap the player with the whatever seal
                Invoke("FadeOut", 1);
            }

            if (GetComponent<SpriteRenderer>().isVisible && !playerNearby)
            {
                playerNearby = true;
                GetComponent<AudioSource>().Play();
                AudioManager.inst.music.volume /= 5;
            }
        }

        if (!GetComponent<SpriteRenderer>().isVisible && playerNearby || isSeal && playerNearby)
        {
            GetComponent<AudioSource>().Pause();
            AudioManager.inst.music.volume *= 5;
            playerNearby = false;
        }
    }

    void FadeOut()
    {
        colourFadeRenderer.FadeObject(gameObject, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController pc))
        {
            player = pc;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController pc))
        {
            if(pc == player)
            {
                player = null;
            }
        }
    }
}
