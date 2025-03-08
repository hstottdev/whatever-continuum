using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchyPlayer : PlayerController
{
    [Header("Head")]
    [SerializeField] HeadController head;

    [Header("Tired Visuals")]

    [SerializeField] RuntimeAnimatorController tiredBodyController;
    RuntimeAnimatorController normalBodyController;

    [SerializeField] RuntimeAnimatorController tiredHeadController;
    RuntimeAnimatorController normalHeadController;

    [SerializeField] Sprite tiredNeckSprite;
    Sprite normalNeckSprite;

    [Header("Guitar")]

    [SerializeField] GameObject guitar;
    [SerializeField] float guitarForce;
    [SerializeField] GameObject arrow;
    [SerializeField] float guitarCooldown = 2.5f;
    float timeSincePlayedGuitar = 0;

    [Header("Hair")]
    [SerializeField] Animator hairAni;
    [SerializeField] RuntimeAnimatorController tiredHairController;
    RuntimeAnimatorController normalHairController;
    [SerializeField] float minHairFlowingVelocity = 0.6f;

    public override void Start()
    {
        base.Start();

        normalHeadController = headAni.runtimeAnimatorController;

        normalNeckSprite = head.neck.sprite;

        normalBodyController = ani.runtimeAnimatorController;

        normalHairController = hairAni.runtimeAnimatorController;

        guitar.SetActive(false);
        timeSincePlayedGuitar = guitarCooldown;
    }

    private void OnEnable()
    {
        onDie.AddListener(head.ResetHead);
    }

    public override bool CanResetRotation()
    {
        return base.CanResetRotation() && !head.bodyReturning;
    }

    public override void RampCheck()
    {
        base.RampCheck();
    }

    public override void SetAniBools()
    {
        base.SetAniBools();
    }

    public override void Update()
    {
        base.Update();

        SetTiredVisuals();

        SetHairVisuals();

        if (dead)
        {
            guitar.SetActive(false);
            arrow.SetActive(false);
        }
    }

    public override void Inputs(bool canInput)
    {
        base.Inputs(canInput);

        if(!canInput)
        {
            return;
        }

        timeSincePlayedGuitar += Time.deltaTime;

        if (Input.GetAxisRaw("Secondary") > 0.1f && timeSincePlayedGuitar > guitarCooldown)
        {
            guitar.SetActive(true);
            guitar.GetComponent<SpriteRenderer>().enabled = true;
            arrow.SetActive(true);
            arrow.GetComponent<SpriteRenderer>().enabled = true;
        }

        if(Input.GetAxisRaw("Secondary") < 0.1f && arrow.activeInHierarchy)
        {
            AudioManager.PlaySound("guitar riff", Random.Range(0.8f, 1.1f), 0.3f);
            guitar.GetComponent<ParticleSystem>().Play();
            rb.AddForce(direction * -transform.right * guitarForce, ForceMode2D.Impulse);
            arrow.SetActive(false);
            timeSincePlayedGuitar = 0;
            Invoke("DisableGuitar", guitarCooldown);
        }


    }

    void SetHairVisuals()
    {
        hairAni.transform.localScale = new Vector2(direction, 1);
        Vector2 hairDirection = new Vector2();

        if (rb.velocity.magnitude > minHairFlowingVelocity || head.stretching)
        {
            if(Mathf.Abs(rb.velocity.x) > minHairFlowingVelocity / 2)
            {
                hairDirection = new Vector2(rb.velocity.x / Mathf.Abs(rb.velocity.x), hairDirection.y);
            }

            if (Mathf.Abs(rb.velocity.y) > minHairFlowingVelocity / 2)
            {
                hairDirection = new Vector2(hairDirection.x, rb.velocity.y / Mathf.Abs(rb.velocity.y));
            }

            if (head.stretching)
            {
                //Debug.Log("rotating hair while head stretches");
                hairDirection = new Vector2(hairDirection.x, head.transform.up.y);
            }

            hairAni.transform.right = hairDirection;

            hairAni.SetBool("rolling", true);
        }
        else
        {
            hairAni.transform.right = head.transform.right;
            hairAni.SetBool("rolling", false);
        }
    }

    public override void Movement()
    {
        base.Movement();
    }

    void DisableGuitar()
    {
        guitar.SetActive(false);
    }

    void SetTiredVisuals()
    {
        bool isTired = !head.canStretch;        

        ani.runtimeAnimatorController = !isTired ? normalBodyController : tiredBodyController;

        headAni.runtimeAnimatorController = !isTired ? normalHeadController : tiredHeadController;

        hairAni.runtimeAnimatorController = !isTired ? normalHairController : tiredHairController;

        head.neck.sprite = !isTired ? normalNeckSprite : tiredNeckSprite;
    }
}
