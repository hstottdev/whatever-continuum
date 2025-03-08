using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cowntroller : PlayerController
{
    [Header("MOOOOOOOOOO")]
    [SerializeField] GameObject mooPrefab;
    [SerializeField] float mooPower = 2;
    [SerializeField] Transform mooSpawnPoint;
    [SerializeField] float mooCooldown;
    float timeSinceMoo;

    [Header("plop")]
    [SerializeField] GameObject pooPrefab;
    [SerializeField] float pooPower = 2;
    [SerializeField] Transform pooSpawnPoint;
    [SerializeField] float pooCooldown;
    float timeSincePoo;

    public override void Start()
    {
        base.Start();

        timeSincePoo = pooCooldown;

        timeSinceMoo = mooCooldown;
    }

    public override void Inputs(bool canInput)
    {
        base.Inputs(canInput);
        
        if(!canInput)
        {
            return;
        }

        Debug.DrawRay(mooSpawnPoint.position, (direction*transform.right) - transform.up / 2, Color.blue);
        if (Input.GetAxisRaw("Primary") > 0.1f && timeSinceMoo > mooCooldown)
        {
            //MOOOOOO
            AudioManager.PlaySound("cow moo",Random.Range(0.8f,1.2f),0.3f);
            Vector2 initialMooVelocity = ((Vector3.Normalize(rb.velocity) / 2 - transform.up) * mooPower);
            SpawnThingAtPlace(mooPrefab, mooSpawnPoint.position, initialMooVelocity);
            timeSinceMoo = 0;
        }

        if (Input.GetAxisRaw("Secondary") > 0.1f && timeSincePoo > pooCooldown)
        {
            //SHID
            AudioManager.PlaySound("fart tick tock",Random.Range(0.8f,1.2f),0.4f);
            SpawnThingAtPlace(pooPrefab, pooSpawnPoint.position, -transform.up * pooPower);
            timeSincePoo = 0;
        }

        timeSinceMoo += Time.deltaTime;
        timeSincePoo += Time.deltaTime;
    }

    public override void SetAniBools()
    {
        base.SetAniBools();
        skateAni.SetBool("rolling", moving);
        ani.SetBool("moving", moving);

        if (dead)
        {
            skateAni.SetBool("equipped", false);
        }

        if (skateAni.GetBool("rolling"))
        {
            skateAni.speed = 1f + Mathf.Abs(rb.velocity.x / 2);
            ani.speed = skateAni.speed;
        }
        else
        {
            skateAni.speed = 1f;
            ani.speed = 1f;
        }
    }

    void SpawnThingAtPlace(GameObject thing, Vector2 place, Vector2 initialVelocity)
    {
        GameObject instance = Instantiate(thing, place, thing.transform.rotation);

        if(instance.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = initialVelocity;
        }
    }
}
