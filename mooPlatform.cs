using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mooPlatform : MonoBehaviour
{
    bool fading;
    bool lerpScale;
    float targetScale;

    [SerializeField] float startScale = 0.2f;
    [SerializeField] float despawnRate = 1;
    [SerializeField] float lerpScaleRate = 1;
    [SerializeField] float maximumLifetime = 100;

    [SerializeField] Collider2D platformColliderOverride;   
    [SerializeField] SpriteRenderer spriteRendererOverride;
    // Start is called before the first frame update
    void Start()
    {
        if(spriteRendererOverride == null)
        {
            spriteRendererOverride = GetComponent<SpriteRenderer>();
        }

        if(platformColliderOverride == null)
        {
            platformColliderOverride = GetComponent<Collider2D>();  
        }
        targetScale = transform.localScale.x;
        transform.localScale = Vector3.one*startScale;
        lerpScale = true;

        Invoke("Despawn", maximumLifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            spriteRendererOverride.color = Color.Lerp(spriteRendererOverride.color, new Color(spriteRendererOverride.color.r,spriteRendererOverride.color.g,spriteRendererOverride.color.b,0),Time.deltaTime*despawnRate);

            if(spriteRendererOverride.color.a < 0.01f)
            {
                Destroy(gameObject);
            }

        }

        if (lerpScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale),Time.deltaTime*lerpScaleRate);
            
            if(targetScale == 0 && transform.localScale.x < 0.2f)
            {
                platformColliderOverride.enabled = false;
            }
            
            if(Mathf.Abs(transform.localScale.x - targetScale) < 0.01)
            {
                lerpScale = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() == null && collision.gameObject.GetComponent<cowPat>() == null)
        {
            Invoke("Despawn", 0.8f);
        }
    }

    void Despawn()
    {
       fading = true;
       lerpScale = true;
       targetScale = 0;
    }
}
