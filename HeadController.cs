using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField] Transform head;
    public SpriteRenderer neck;
    [SerializeField] PlayerController player;
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] float riseSpeed = 10;
    [SerializeField] float bodyCatchupSpeed = 1;
    [SerializeField] float maxHeight = 5;
    float defaultHeadHeight;
    Vector2 oldPlayerVelocity;
    [HideInInspector] public bool stretching;
    [HideInInspector] public bool bodyReturning;
    [HideInInspector] public bool canStretch;
    float originalPlayerGravity;
    bool stunned;
    bool failedStretch;
    [SerializeField] AudioSource stretchNoise;

    // Start is called before the first frame update
    void Start()
    {        
        defaultHeadHeight = head.localPosition.y;
        originalPlayerGravity = playerBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        bool canInput = (!player.dead && !ButtonManager.paused);
        Inputs(canInput);
    }

    void Inputs(bool canInput)
    {
        if(!canInput)
        {
            StopStretching();
            return;
        }

        if(Input.GetAxisRaw("Primary") > 0.1f)//trying to stretch
        {
            if(canStretch && !stunned)//can stretch
            {
                if(!stretching) stretching = true;
            }
            else if(!failedStretch)//trying to stretch when can't
            {
                failedStretch = true;
                AudioManager.PlaySound("poosplosion", Random.Range(1.8f,2f),0.3f);
            }
        }
        else
        {
            failedStretch = false;
        }

        if (FullyStretched())
        {
            StopStretching();
        }

        //if let go of button, return the body to the head
        if (Input.GetAxisRaw("Primary") < 0.1f && !headAtDefaultHeight() && headHasExtended() && !bodyReturning)
        {
            StartReturnBody();
        }
    }

    bool headAtDefaultHeight()
    {
        return !(Mathf.Abs(head.localPosition.y - defaultHeadHeight) > 0.1f);
    }

    bool headHasExtended()
    {
        return head.localPosition.y > defaultHeadHeight;
    }

    void StartReturnBody()
    {
        StopStretching();
        bodyReturning = true;
        canStretch = false;
        oldPlayerVelocity = new Vector2(playerBody.velocity.x, 0);
        playerBody.velocity = GetBodyReturnVelocity() + oldPlayerVelocity;
    }

    void StopStretching()
    {
        if (stretching)
        {
            stretching = false;
            canStretch = false;
        }
    }

    bool FullyStretched()
    {
        return (head.localPosition.y > maxHeight);
    }

    private void FixedUpdate()
    {
        Movement();

        float zAngle = 0;
        //Debug.Log(zAngle);
        Quaternion targetRotation = new Quaternion(transform.rotation.x, transform.rotation.y, zAngle, transform.rotation.w);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5*Time.deltaTime);
    }

    public void Movement()
    {
        //Debug.Log(Mathf.Abs(head.localPosition.y - defaultHeadHeight));

        if (stretching)
        {
            playerBody.gravityScale = originalPlayerGravity;
            head.localPosition = new Vector2(head.localPosition.x, head.localPosition.y + Time.deltaTime * riseSpeed);
            UpdateNeckSize();

            if(stretchNoise != null)
            {
                if(!stretchNoise.isPlaying) stretchNoise.Play();
                stretchNoise.pitch = Mathf.Abs(stretchNoise.pitch);
            }
        }
        else if (bodyReturning)
        {
            if(!headAtDefaultHeight() && headHasExtended())
            {
                playerBody.gravityScale = 0;
                ReturnBody();

                if(stretchNoise != null)
                {
                    stretchNoise.pitch = -Mathf.Abs(stretchNoise.pitch);
                    if(!stretchNoise.isPlaying) stretchNoise.Play();
                }   

            }
            else
            {
                StopReturnBody();
                playerBody.gravityScale = originalPlayerGravity;
            }
        }
        else
        {
            playerBody.gravityScale = originalPlayerGravity;
            SetCanStretch();
        }
    }

    void SetCanStretch()
    {
        if (FullyStretched())
        {
            canStretch = false;
        }
        else if (player.grounded)
        {
            canStretch = true;
        }
    }

    void ReturnBody()
    {
        //Debug.Log("head position difference " + Mathf.Abs(head.localPosition.y - defaultHeadHeight));
        head.Translate(-GetBodyReturnVelocity() * Time.deltaTime, Space.World);
        head.localPosition = new Vector3(0, head.localPosition.y, head.localPosition.z);
        UpdateNeckSize();
    }

    Vector2 GetBodyReturnVelocity()
    {
        return (Vector2)playerBody.transform.up * bodyCatchupSpeed;
    }

    void StopReturnBody()
    {
        if (bodyReturning)
        {
            bodyReturning = false;

        }
    }

    public void ResetHead()
    {
        head.localPosition = new Vector2(head.localPosition.x, defaultHeadHeight);
        stretching = false;
        bodyReturning = false;
        UpdateNeckSize();
    }

    void UpdateNeckSize()
    {
        neck.size = new Vector2(neck.size.x, head.localPosition.y + 0.3f);

        BoxCollider2D neckCollider = neck.GetComponent<BoxCollider2D>();
        neckCollider.size = new Vector2(neckCollider.size.x,neck.size.y);
        neckCollider.offset = new Vector2(neckCollider.offset.x, neckCollider.size.y / 2);
    }


    void Stun()
    {
        stunned = true;
    }

    void Recover()
    {
        stunned = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        bool ceilingHit = collision.contacts[0].normal == new Vector2(0, -1);
        //Debug.Log(collision.gameObject.layer);
        if (stretching && collision.gameObject.layer == LayerMask.NameToLayer("Ground") && ceilingHit) 
        {
            //Debug.Log("bonk");
            //Debug.Log("Ceiling Hit: "+ceilingHit);
            StopStretching();
            Stun();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Invoke("Recover", 0.5f);
        }
    }
}
