using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cowPat : MonoBehaviour
{
    [SerializeField] ContactFilter2D contactFilter;
    [SerializeField] float animationWaitTime = 1;
    [SerializeField] float explosionWaitTime = 1.5f;
    [SerializeField] float explosionRange;
    [SerializeField] float explosionPower;
    [SerializeField] GameObject explosionVisuals;
    [SerializeField] float mooPlatformLaunchPower;
    GameObject animationObj;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("PlayAnimation", animationWaitTime);
        Invoke("Explode", explosionWaitTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(animationObj != null)
        {
            animationObj.transform.rotation = Quaternion.identity;
        }
    }

    void PlayAnimation()
    {
        animationObj = Instantiate(explosionVisuals, transform.position, explosionVisuals.transform.rotation);
        animationObj.transform.SetParent(transform);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Explode()
    {      
        AudioManager.PlaySound("poosplosion",Random.Range(0.8f,1.2f));

        List<Collider2D> overlappers = new List<Collider2D>(15);

        Physics2D.OverlapCircle(transform.position, explosionRange,contactFilter, overlappers);

        foreach(Collider2D c in overlappers)
        {
            if (c.attachedRigidbody != null && c.gameObject != gameObject)
            {
                Rigidbody2D rb = c.attachedRigidbody;
                //Debug.Log($"found {rb.gameObject.name} in explosion range");
                rb.velocity = Vector2.zero;
                rb.AddForce(explosionPower * Vector3.Normalize(rb.transform.position - transform.position), ForceMode2D.Impulse);

                if(rb.gameObject.tag == "moo")
                {
                    rb.velocity = mooPlatformLaunchPower * Vector3.Normalize(rb.transform.position - transform.position);
                }
            }

            if(c.TryGetComponent(out HealthManager hm))
            {
                hm.Transaction(-20,0.1f);
            }
        }

        if(animationObj != null)
        {
            animationObj.transform.SetParent(null);
        }

        Destroy(gameObject);
    }


}
