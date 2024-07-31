using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boinger : MonoBehaviour
{
    [SerializeField] float boingForce;
    [SerializeField] float offscreenDespawnDelay;
    float timeSinceOnScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<SpriteRenderer>().isVisible)
        {
            timeSinceOnScreen = 0;
        }
        else
        {
            timeSinceOnScreen += Time.deltaTime;
        }

        if(timeSinceOnScreen > offscreenDespawnDelay)
        {
            Destroy(gameObject);
            Debug.Log("destroy after offscreen");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
         if(collision.gameObject.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(boingForce * Vector3.Normalize(rb.transform.position - transform.position), ForceMode2D.Impulse);

            if(GetComponent<SpriteRenderer>() != null)
            {
                if (GetComponent<SpriteRenderer>().isVisible)
                {
                    AudioManager.PlaySound("weird boing",Random.Range(0.4f,0.6f),0.2f);
                }
            }

        }

        if(collision.gameObject.GetComponentInChildren<HeadController>() != null)
        {
            collision.gameObject.GetComponentInChildren<HeadController>().canStretch = true;
        }

        
    }
}
