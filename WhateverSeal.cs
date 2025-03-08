using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WhateverSeal : PlayerController
{
    [Header("Seal")]
    [SerializeField] float turnLerpSpeed = 6;
    [SerializeField] float rolloverLerpSpeed = 2;
    [SerializeField] float upperRolloverLimit = 300;
    [SerializeField] float lowerRolleroverLimit = 140;
    [SerializeField] float rolloverDelay = 0.8f;
    [SerializeField] GameObject particleParent;
    Vector2 inputDirection;
    bool showingTrail;
    float timeBeenBadFloppage;
    [Header("Vortex")]
    [SerializeField] AudioClip vortexMusic;
    [SerializeField] GameObject vortexSeal;
    [SerializeField] float vortexSealScale = 6;
    [SerializeField] GameObject vortex;
    bool vortexMode;

    public override void Start()
    {
        base.Start();
        AudioManager.inst.music.time = 0;
        AudioManager.inst.music.loop = false;

        foreach (ParticleSystem ps in particleParent.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
            showingTrail = false;
        }
        LevelManager.mode = LevelMode.whatever;
        inputDirection = transform.right;
    }

    public override void Movement()
    {
        //base.Movement();

        Vector3 forceDirection = inputDirection;
        float forceIntensity = movementForce;
        float maxSpeed = maxMoveSpeed;

        if (skating && !vortexMode)
        {
            forceIntensity = skateForce;
            maxSpeed = maxSkateSpeed;
        }

        if (moving)
        {
            Console.SpeedLimitedForce(forceDirection, forceIntensity, maxSpeed, rb, ForceMode2D.Force);

            if (!showingTrail)
            {
                foreach (ParticleSystem ps in particleParent.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                    showingTrail = true;
                }
            }

        }
        else
        {
            if (showingTrail)
            {
                foreach (ParticleSystem ps in particleParent.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Stop();
                    showingTrail = false;
                }
            }
        }

        SealFlipCheck();
    }

    void SealFlipCheck()
    {
        Vector3 targetScale;
        //Debug.Log(transform.rotation.eulerAngles.z);

        if (transform.rotation.eulerAngles.z < lowerRolleroverLimit || transform.rotation.eulerAngles.z > upperRolloverLimit)
        {
            timeBeenBadFloppage += Time.deltaTime;
            targetScale = Vector3.one;
        }
        else
        {
            timeBeenBadFloppage += Time.deltaTime;
            targetScale = new Vector3(1, -1, 1);
        }

        if(timeBeenBadFloppage > rolloverDelay)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * rolloverLerpSpeed);
        }

        if(Vector3.Distance(transform.localScale,targetScale) < 0.1f)
        {
            timeBeenBadFloppage = 0;
        }
    }

    public override void Inputs(bool canInput)
    {
        if (vortexMode) canInput = false;
        
        if(!canInput)
        {
            moving = false;
            inputDirection = Vector3.right;
        }
        else
        {
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (inputDirection.magnitude > 0f)
            {
                moving = true;
                Debug.DrawRay(transform.position, inputDirection, Color.red);
                //transform.LookAt(direction);
            }
            else
            {
                moving = false;
            }
            skating = Input.GetButton("Sprint");
        }

        if (Vector2.Distance(transform.right, inputDirection) > 0.1f)//only lerp if the distance from the correct direction is great enough
        {
            transform.right = Vector2.Lerp(transform.right, inputDirection, Time.deltaTime * turnLerpSpeed);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //overwrite regular collision stay
    }

    public IEnumerator EnterVortex()
    {
        ToggleSpriteAndCollider(false, true);
        GameObject newSeal = Instantiate(vortexSeal, transform.position, Quaternion.identity);
        AudioManager.SetMusic(vortexMusic,0.8f);
        vortexMode = true;

        bool notInMiddle()
        {
            return Vector2.Distance(newSeal.transform.position, Camera.main.transform.position) > 0.03f;
        }

        bool notBigBoy()
        {
            return Vector3.Distance(newSeal.transform.localScale, Vector3.one * vortexSealScale) > 0.03f;
        }

        if (UiManager.instance != null) UiManager.instance.vortexVoid.gameObject.SetActive(true);

        while (notInMiddle())
        {
            newSeal.transform.position = Vector2.Lerp(newSeal.transform.position, Camera.main.transform.position, 2 * Time.deltaTime);

            yield return null;
        }
        yield return new WaitWhile(() => AudioManager.inst.music.time < 3);

        Instantiate(vortex, newSeal.transform.position, Quaternion.identity);

        if (UiManager.instance != null)
        {
            UiManager.instance.winScreen.SetActive(true);

            InputChecker.startButton = UiManager.instance.continueButton;
            InputChecker.SelectFirstActiveButton();
        }

        float lerpSpeed = 1.4f;
        while (notBigBoy())
        {
            lerpSpeed = Mathf.Pow(lerpSpeed, 1.1f);
            newSeal.transform.localScale = Vector3.Lerp(newSeal.transform.localScale, Vector3.one * vortexSealScale, lerpSpeed*Time.deltaTime);
            yield return null;
        }
    }
}
