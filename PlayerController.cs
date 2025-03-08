using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    public AudioClip music;
    public string jumpSFX;
    [SerializeField] GameObject ghostPrefab;

    [Header("Slopes")]
    [SerializeField] float angleLerpSpeed = 7.5f;
    [SerializeField] float rampCastDistance = 1;
    [SerializeField] Vector3 rampCastOffset;
     Vector2 currentTargetDirection;
    [Header("Animation")]
    public Animator ani;
    public Animator skateAni;
    public Animator headAni;
    [Header("Particles")]
    [SerializeField] ParticleSystem runningParticles;
    [SerializeField] ParticleSystem midairParticles;
    [SerializeField] ParticleSystem skatingParticles;

    [Header("Physics")]
    public float movementForce = 36;
    public float maxMoveSpeed = 6;
    [SerializeField] float jumpPower = 7.5f;
    [SerializeField] float jumpCooldown = 0.2f;
    [SerializeField] float crouchJumpPower = 7.5f;
    [SerializeField] float crouchJumpTime = 0.8f;
    [SerializeField] PhysicsMaterial2D walkingPhysics;
    [SerializeField] PhysicsMaterial2D slippyPhysics;

    [Header("Skating")]
    public bool canSkate = true;
    public bool skateIsToggle;
    public float skateForce;
    public float maxSkateSpeed;
    [SerializeField] float maxAngle = 50;
    [SerializeField] float rampLerpSpeed = 4;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float groundRayCastDistance = 0.3f;
    [SerializeField] Vector3 groundRaycastOffset;
    [SerializeField] float edgeCastModifier = 2;

    [Header("Death")]
    [SerializeField] float ghostAccelaration = 1;
    [HideInInspector] public bool dead;
    [HideInInspector] public Transform currentGhost;

    float timeWithoutJumping;
    [HideInInspector] public bool moving;
    [HideInInspector] public int direction = 1;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public bool jumping;
    [HideInInspector] public bool crouchJumping;
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool skating;
    [HideInInspector] public bool crouched;
    public static Vector2 respawnPoint;
    [HideInInspector] public UnityEvent onDie;
    // Start is called before the first frame update
    public virtual void Start()
    {
        try
        {
            SetVirtualCamFollow();
        }
        finally
        {
            rb = GetComponent<Rigidbody2D>();

            if (respawnPoint == null)
            {
                respawnPoint = transform.position;
            }
            instance = this;

            if(music != null)
            {
                AudioManager.SwitchMusicSamePosition(music);
                AudioManager.inst.music.loop = true;
            }
        }
    }

    void SetVirtualCamFollow()
    {
        CinemachineVirtualCamera virtualCamera = GameObject.FindGameObjectWithTag("virtualCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        grounded = CanJump();
        
        bool canInput = !dead && !ButtonManager.paused && !LevelManager.lost && !LevelManager.disableInputs;
        
        Inputs(canInput);

        //set visual states
        SetAniBools();
        SetParticleConditions();
        

        timeWithoutJumping += Time.deltaTime;
    }

    //Checking when inputs begin
    public virtual void Inputs(bool canInput)
    {     
        if(!canInput)
        {
            moving = false;
            jumping = false;
            crouched = false;
            return;
        }

        //right
        if (Input.GetAxisRaw("Horizontal") <= -0.4f)
        {
            moving = true;
            if (direction == 1)
            {
                direction = -1;
                UpdateDirection();
            }
        }

        //left
        if (Input.GetAxisRaw("Horizontal") >= 0.4f)
        {
            moving = true;
            if (direction == -1)
            {
                direction = 1;
                UpdateDirection();
            }
        }

        //no longer moving
        bool letGoMoving = Input.GetAxisRaw("Horizontal") >= -0.2f && Input.GetAxisRaw("Horizontal") <= 0.2f;
        if (moving && letGoMoving)
        {
            moving = false;
        }

        //beginning jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (crouched && grounded)//crouch jump
            {
                crouchJumping = true;
                rb.velocity = Vector3.zero;

                Invoke("DisableCrouchJump", crouchJumpTime);

                ani.Play("Crouch Jump");
            }
            else//regular jump
            {
                jumping = true;
            }
        }

        //no longer jumping
        bool letGoJumping = Input.GetButtonUp("Jump");
        if (jumping && letGoJumping)
        {
            jumping = false;
        }

        //skating
        if (canSkate)
        {
            if (skateIsToggle)
            {
                if (Input.GetButtonDown("Sprint"))
                {
                    skating = !skating;
                }
            }
            else
            {
                skating = Input.GetButton("Sprint");
            }
        }

        //down directional input (crouching)
        if (Input.GetAxisRaw("Vertical") <= -0.7f && !crouched)
        {
            crouched = true;
        }

        //no longer crouching
        bool letGoCrouching = Input.GetAxisRaw("Vertical") > -0.7f && crouched;
        if (crouched && letGoCrouching)
        {
            crouched = false;
        }
    }

    void FixedUpdate()
    {
        SetPhysicsMaterials();

        if (LevelManager.lost || LevelManager.disableInputs)
        {
            moving = false;
        }
        else
        {
            Movement();
        }
    }

    public void ToggleSpriteAndCollider(bool toggleSprite,bool toggleCollider)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.TryGetComponent(out SpriteRenderer s))
            {
                s.enabled = toggleSprite;
            }

            if (t.TryGetComponent(out Collider2D c))
            {
                c.enabled = toggleCollider;
            }
        }
    }

    public IEnumerator Respawn()
    {
        currentGhost = Instantiate(ghostPrefab).transform;
        currentGhost.position = transform.position;
        dead = true;      
        AudioManager.inst.music.volume /=5;
        moving = false;
        bool wasSkating = skating;
        skating = false;
        rb.velocity = Vector2.zero;
        jumping = false;
        float currentLerpSpeed = 0;
        //float deathDistance = Vector2.Distance(ghost.position, respawnPoint);

        ToggleSpriteAndCollider(false,false);

        while (Vector2.Distance(currentGhost.position, respawnPoint) > 0.5f)
        {
            currentLerpSpeed += ghostAccelaration;
            currentGhost.position = Vector2.MoveTowards(currentGhost.position, respawnPoint, 0.02f * currentLerpSpeed);
            transform.position = currentGhost.position;

            if (Input.anyKeyDown)
            {
                //Debug.Log("tried spamming");
                currentLerpSpeed *= 2f;
            }
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(currentGhost.gameObject);
        transform.position = respawnPoint;
        dead = false;
        AudioManager.inst.music.volume = AudioManager.inst.defaultMusicVolume;
        ToggleSpriteAndCollider(true,true);

        skating = wasSkating;

        try
        {
            onDie.Invoke();
        }
        catch
        {
            Debug.LogError("Error when invoking onDie event");
        }
    }

    void SetPhysicsMaterials()
    {
        if (skating || !grounded)
        {
            rb.sharedMaterial = slippyPhysics;
        }
        else
        {
            rb.sharedMaterial = walkingPhysics;
        }
    }

    void UpdateDirection()
    {
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    public virtual void Movement()
    {
        if (moving && !crouchJumping)
        {
            Vector2 forceDirection;
            if (grounded)
            {
                forceDirection = transform.right;
            }
            else
            {
                forceDirection = Vector2.right;
            }

            float forceIntensity;
            float maxSpeed;
            if (skating && grounded)
            {
                forceIntensity = skateForce * direction;
                maxSpeed = maxSkateSpeed;
            }
            else
            {
                forceIntensity = movementForce * direction;
                maxSpeed = maxMoveSpeed;
            }

            Console.SpeedLimitedForce(forceDirection, forceIntensity, maxSpeed, rb, ForceMode2D.Force);
        }

        if (jumping && grounded && timeWithoutJumping > jumpCooldown)
        {
            Jump(jumpPower,ForceMode2D.Impulse);
        }
        if (crouchJumping)
        {
            Jump(crouchJumpPower, ForceMode2D.Force);
        }
        RampCheck();
    }

    public virtual void Jump(float power,ForceMode2D forceMode)
    {
        rb.AddForce(rb.transform.up * power, ForceMode2D.Impulse);
        timeWithoutJumping = 0;

        if(jumpSFX != null)
        {
            AudioManager.PlaySound(jumpSFX, 1,0.5f);
        }
    }

    void DisableCrouchJump()
    {
        crouchJumping = false;
    }

    bool CanJump()
    {
        Vector3 rayPosition = transform.position + groundRaycastOffset;
        Vector3 edgeOffset = new Vector3();

        if (TryGetComponent(out CapsuleCollider2D capsuleCollider2D))
        {
            edgeOffset = new Vector3(capsuleCollider2D.size.x / edgeCastModifier, 0, 0);
        }

        if (TryGetComponent(out BoxCollider2D boxCollider2D))
        {
            edgeOffset = new Vector3(boxCollider2D.size.x / edgeCastModifier, 0, 0);
        }


        bool downHit = Physics2D.Raycast(rayPosition, -Vector3.up, groundRayCastDistance, groundLayers);
        bool downRightHit = Physics2D.Raycast(rayPosition + edgeOffset, -Vector3.up, groundRayCastDistance, groundLayers);
        bool downLeftHit = Physics2D.Raycast(rayPosition - edgeOffset, -Vector3.up, groundRayCastDistance, groundLayers);

        //Debug.DrawRay(rayPosition + edgeOffset, -Vector3.up * groundRayCastDistance, Color.green);
        //Debug.DrawRay(rayPosition - edgeOffset, -Vector3.up * groundRayCastDistance, Color.red);
        //Debug.DrawRay(rayPosition, (Vector2.down) * groundRayCastDistance, Color.blue);

        return downHit || downRightHit || downLeftHit;
    }

    public virtual void RampCheck()
    {
        Vector3 rayPosition = transform.position + rampCastOffset;
        Vector3 edgeOffset = new Vector3(0.1f, 0, 0);

        RaycastHit2D ray = Physics2D.Raycast(rayPosition + direction * edgeOffset, -Vector2.up,rampCastDistance, groundLayers);
        Debug.DrawRay(rayPosition + direction * edgeOffset, -Vector2.up * rampCastDistance, Color.magenta);
        
        if (ray != null)
        {
            currentTargetDirection = ray.normal;

            float slopeAngle = GetCurrentAngle();

            if (slopeAngle < maxAngle)
            {
                Vector2 dampVelocity = new Vector2();
                transform.up = Vector2.SmoothDamp(transform.up, currentTargetDirection, ref dampVelocity, Time.deltaTime * rampLerpSpeed, angleLerpSpeed);
            }
        }

        if (CanResetRotation())
        {
            transform.up = Vector2.Lerp(transform.up, Vector2.up, Time.deltaTime * 5);
        }

        //Physics2D.gravity = -transform.up * 9.81f;
    }

    float GetCurrentAngle()
    {
        return Vector3.Angle(Vector3.up, currentTargetDirection);
    }

    public virtual bool CanResetRotation()
    {
        return !grounded && !crouchJumping;
    }

    public virtual void SetAniBools()
    {
        ani.SetBool("moving", moving && !skating);
        ani.SetBool("grounded", grounded || skating);
        ani.SetBool("crouched", crouched && !moving  || crouched && skating);
        ani.SetBool("crouchJump", crouchJumping);

        if (skateAni != null)
        {
            skateAni.SetBool("equipped", skating);

            if (skateAni.GetBool("rolling"))
            {
                skateAni.speed = 1f + Mathf.Abs(rb.velocity.x / 2);
            }
            else
            {
                skateAni.speed = 1f;
            }
        }

        if (headAni != null)
        {
            headAni.SetBool("rolling", skating && Mathf.Abs(rb.velocity.x) > 1f);
            if (headAni.GetBool("rolling"))
            {
                headAni.speed = Mathf.Abs(rb.velocity.x / 3);
            }
            else
            {
                headAni.speed = 1f;
            }

            headAni.SetBool("crouched", crouched);
        }     

    }

    void SetSprintFX()
    {
        if(UiManager.instance != null)
        {
            ParticleSystem sprintPS = UiManager.instance.sprintFX;
            if (skating && Mathf.Abs(rb.velocity.x) > 5)
            {
                sprintPS.startSpeed = 10*Mathf.Abs(rb.velocity.x);

                if (rb.velocity.x < 0)
                {
                    sprintPS.transform.localScale = new Vector2(-1, 1);
                }
                else
                {
                    sprintPS.transform.localScale = new Vector2(1, 1);
                }
                if (!sprintPS.isPlaying)
                {
                    sprintPS.Play();

                    if(sprintPS.transform.childCount > 0)
                    {
                        sprintPS.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (sprintPS.isPlaying)
                {
                    sprintPS.Stop();

                    if (sprintPS.transform.childCount > 0)
                    {
                        sprintPS.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void SetParticleConditions()
    {
        //running
        Console.SetParticles(runningParticles, Mathf.Abs(rb.velocity.magnitude) > 0.1f && grounded && !skating && !dead);
        //mid-air
        Console.SetParticles(midairParticles, !grounded && !skating && !dead);
        if(skateAni != null)
        {
            bool skatingAnimation = skateAni.GetCurrentAnimatorStateInfo(0).IsName("Rolling");
            Console.SetParticles(skatingParticles, Mathf.Abs(rb.velocity.magnitude) > 1f && skatingAnimation && grounded && !dead);
        }


        SetSprintFX();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazard") && !dead)
        {
            StartCoroutine(Respawn());
        }
    }
}
