using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveTowardForce;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float chargeCancelTime = 3;
    Animator ani;
    bool charging;
    bool cancelledCharge;

    [Header("Vision")]
    [SerializeField] LayerMask visibleLayers;
    [SerializeField] float visionRange;
    [SerializeField] Vector2 visionOrigin;

    [Header("rESPAWNIGN")]
    [SerializeField] float respawnDistance;

    [Header("Death")]
    [SerializeField] GameObject explosion;


    SpriteRenderer rend;
    bool respawned;
    Vector2 originalPosition;
    Quaternion originalRotation;

    Rigidbody2D rb;
    int direction;

    private void Reset()
    {
        respawnDistance = 20;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        respawned = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        ani = GetComponent<Animator>();
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{gameObject.name}: Player in sight range: {CanSeePlayer()}");

        ChargeCheck();//check for whether to charge at the player

        RespawnCheck();//check whether to reset the position of the enemy

        DirectionCheck();

/*        if (Input.GetKeyDown("b"))
        {
            Explode();
        }*/
    }

    private void FixedUpdate()
    {
        if (charging)
        {
            Movement();//movement while charging
        }
    }

    void DirectionCheck()
    {
        bool withinRange = Mathf.Abs(transform.position.x - PlayerController.instance.transform.position.x) < visionRange;
        bool playerBehind = PlayerController.instance.transform.position.x < transform.position.x;
        if (withinRange)
        {
            if (playerBehind && direction == 1)
            {
                direction = -1;
                UpdateDirection();
            }

            if(!playerBehind && direction == -1)
            {
                direction = 1;
                UpdateDirection();
            }
        }
    }

    void UpdateDirection()
    {
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    void ChargeCheck()
    {
        //Debug.Log($"behind: {");
        if (CanSeePlayer())
        {
            if (!charging)//if you can see the player and you're not charging
            {
                StartCharging();//start charging
            }
        }
        else
        {
            if (charging)//if you can't see player and you are charging
            {
                Invoke("StopCharging", chargeCancelTime);//stop charging after 'x' seconds 
            }
        }
    }

    void RespawnCheck()
    {
        Transform playerTransform = PlayerController.instance.transform;

        bool awayFromMe = Vector2.Distance(playerTransform.position, transform.position) > respawnDistance;//is the player far enough from the enemy?
        bool awayFromRespawnPoint = Vector2.Distance(playerTransform.position, originalPosition) > respawnDistance;//is the player far enough from the enemies respawn point

        if (rend.isVisible && respawned)
        {
            respawned = false;
        }

        if(awayFromMe && awayFromRespawnPoint && !respawned)
        {
            if (!rend.isVisible)
            {
                Respawn();
            }
        }
    }
    void StartCharging()
    {
        ani.SetBool("moving", true);
        charging = true;
    }

    void StopCharging()
    {
        if (charging && !CanSeePlayer())
        {
            ani.SetBool("moving", false);
            charging = false;
        }
    }

    void Respawn()
    {
        Debug.Log($" {gameObject.name} RESPAWNED");
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.velocity = Vector2.zero;
        respawned = true;
    }

    void Movement()
    {
        Console.SpeedLimitedForce(transform.right, moveTowardForce*direction, maxMoveSpeed, rb, ForceMode2D.Force);
    }

    bool CanSeePlayer()
    {
        RaycastHit2D ray = Physics2D.Raycast(GetVisionPosition(), transform.right*direction, visionRange, visibleLayers);
        Debug.DrawRay(GetVisionPosition(), transform.right * visionRange* direction, Color.red);

        if(ray.collider != null)
        {
            //Debug.Log($"enemy hit {ray.collider.gameObject}");
            return ray.collider.GetComponentInParent<PlayerController>() != null;
        }
        else
        {
            return false;
        }

    }

    Vector3 GetVisionPosition()
    {
        return (Vector2)transform.position + visionOrigin;
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, explosion.transform.rotation);

        Destroy(gameObject,0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Hazard") || collision.gameObject.GetComponent<WhateverSeal>() != null)
        {
            Kill();
        }
    }

    public void Kill()
    {
            if(TryGetComponent(out HealthManager hm))
            {
                hm.Kill();
            }
    }

}
