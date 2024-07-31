using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class spring : MonoBehaviour
{
    [SerializeField] float springForce = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody != null)
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(springForce * transform.up, ForceMode2D.Impulse);

            if(GetComponent<SpriteRenderer>() != null)
            {
                if (GetComponent<SpriteRenderer>().isVisible)
                {
                    AudioManager.PlaySound("weird boing",Random.Range(0.9f,1.1f),0.6f);
                }
            }

        }

        if(collision.gameObject.GetComponentInChildren<HeadController>() != null)
        {
            collision.gameObject.GetComponentInChildren<HeadController>().canStretch = true;
        }
    }
}
