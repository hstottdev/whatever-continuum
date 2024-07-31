using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dimensionalObject : MonoBehaviour
{
    [SerializeField] string objectKey;
    [SerializeField] Dimension currentDimension;
    RuptureManager ruptureManager;
    [HideInInspector] public bool swapping;
    [SerializeField] float dimSwapSpeed = 30;

    public void SwapDimension()
    {
        if(swapping) return;
        //Debug.Log("trying swap for "+gameObject.name);
        Dimension newDimension;
        if(currentDimension == ruptureManager.mainDimension)
        {
            newDimension = ruptureManager.ruptureDimension;
        }
        else
        {
            newDimension = ruptureManager.mainDimension;
        }
        try
        {
            GameObject newPrefabObject = null;
            if (objectKey != "player")
            {
                newPrefabObject = newDimension.objects[objectKey];
            }
            else
            {                
                if(TryGetComponent(out PlayerController pc))
                {
                    if (!pc.dead)
                    {
                        newPrefabObject = newDimension.player.gameObject;
                    }
                }
                else
                {
                    newPrefabObject = newDimension.player.gameObject;
                }
            }

            StartCoroutine(ScaleLerpSwap(newPrefabObject));
        }
        catch
        {
            Debug.LogError($"Unable to swap object with key:{objectKey} for dimension {newDimension}");
        }
    }

    int GetDirection(GameObject obj)
    {
        return (int)obj.transform.localScale.x < 0 ? -1 : 1;
    }

    public IEnumerator ScaleLerpSwap(GameObject newPrefabObject)
    { 
        swapping = true;
        int oldDirection = GetDirection(gameObject);
        //lerp scale of me to zero
        while(Vector3.Distance(transform.localScale,Vector3.zero) > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero,Time.deltaTime * dimSwapSpeed);
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = Vector3.zero;

        //spawn new object
        GameObject newInstance = Instantiate(newPrefabObject, transform.position, transform.rotation, transform.parent);
        newInstance.transform.localScale = Vector3.zero;
    

        Vector3 targetScale = new Vector3(oldDirection * newPrefabObject.transform.localScale.x, newPrefabObject.transform.localScale.y, newPrefabObject.transform.localScale.z);

        //lerp scale of new object to its default scale
        while (Vector3.Distance(newInstance.transform.localScale,targetScale) > 0.1f)
        {
            newInstance.transform.localScale = Vector3.Lerp(newInstance.transform.localScale, targetScale, Time.deltaTime * dimSwapSpeed);
            yield return new WaitForEndOfFrame();
        }
        newInstance.transform.localScale = targetScale;
        ApplyInstanceVariables(newPrefabObject, newInstance);
        swapping = false;
        Destroy(gameObject);
    }

    void ApplyInstanceVariables(GameObject prefab,GameObject newInstance)
    {
        if(newInstance.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            newInstance.transform.rotation = transform.rotation;
        }

        if(newInstance.TryGetComponent(out PlayerController pc))
        {
            if (pc.GetComponent<WhateverSeal>() != null)//if its the whatever seal
            {
                newInstance.transform.localScale = new Vector3(prefab.transform.localScale.x,-1, prefab.transform.localScale.z);
                newInstance.transform.rotation = prefab.transform.rotation;
            }
            else
            {
                pc.direction = GetDirection(newInstance);
                
                //if skating toggled before, I am still skating
                if(GetComponent<PlayerController>() != null)
                {
                    PlayerController oldPlayer = GetComponent<PlayerController>();
                    if(oldPlayer.skateIsToggle) pc.skating = oldPlayer.skating;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rupture") && ruptureManager == null)
        {
            ruptureManager = collision.gameObject.GetComponent<RuptureManager>();//setting rupture manager variable
            ruptureManager.onSwap.RemoveListener(SwapDimension);
            ruptureManager.onSwap.AddListener(SwapDimension);//adding event listener
            //Debug.Log($"{gameObject.name} In Rupture");


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Rupture"))
        {
            if(ruptureManager != null)
            {
                ruptureManager.onSwap.RemoveListener(SwapDimension);
                ruptureManager = null;
            }
            //Debug.Log($"{gameObject.name} Out Of Rupture");
        }
    }
}
