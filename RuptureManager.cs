using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RuptureManager : MonoBehaviour
{
    public static int count;
    [SerializeField] bool visualOnly;
    Dictionary<Dimension, SpriteRenderer> activeBackgrounds = new Dictionary<Dimension,SpriteRenderer>();
    [HideInInspector] public Dimension currentDimension;
    [SerializeField] float swapDelay = 1;
    public Dimension mainDimension;
    public Dimension ruptureDimension;
    [SerializeField] Grid mainTilemap;
    [SerializeField] Grid ruptureTilemap;
    public UnityEvent onSwap = new UnityEvent();


    [Header("Closing")]
    [SerializeField] float closeSpeed = 3;
    [HideInInspector] public bool closed;
    [SerializeField] GameObject closingFx;
    Vector2 closedScale;

    [Header("Opening")]
    [SerializeField] bool startClosed;
    Vector2 openScale;
    // Start is called before the first frame update
    void Start()
    {
        openScale = transform.localScale;
        closedScale = new Vector3(0, transform.localScale.y, transform.localScale.z);

        currentDimension = mainDimension;

        if (!visualOnly)
        {
            count++;

            InvokeRepeating("Swap", swapDelay, swapDelay);
        }
        if (startClosed)
        {
            closed = true;
            transform.localScale = closedScale;
        }
    }

    SpriteRenderer GetDimensionBackground(Dimension dimension)
    {
        if(dimension == mainDimension)
        {
            try
            {
                return GameObject.FindGameObjectWithTag("MainBackground").GetComponent<SpriteRenderer>();
            }
            catch
            {
                Debug.LogError("No object found tagged with 'MainBackground' ");
                return null;
            }
        }
        else
        {
            SpriteRenderer background;
            if (activeBackgrounds.TryGetValue(dimension, out background))
            {
                return background;
            }
            else//if it hasn't already been spawned
            {
                background = Instantiate(dimension.background, Camera.main.transform).GetComponent<SpriteRenderer>();//spawn it and parent to camera
                activeBackgrounds.Add(dimension, background);//add to dict
                return background;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!visualOnly)
        {
            if (currentDimension == mainDimension)
            {
                SetTileMapMask(mainTilemap, SpriteMaskInteraction.None);
                GetDimensionBackground(mainDimension).maskInteraction = SpriteMaskInteraction.None;

                GetDimensionBackground(ruptureDimension).gameObject.SetActive(false);
                ruptureTilemap.gameObject.SetActive(false);
            }
            else if (currentDimension == ruptureDimension)
            {
                SetTileMapMask(mainTilemap, SpriteMaskInteraction.VisibleOutsideMask);
                GetDimensionBackground(mainDimension).maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                ruptureTilemap.gameObject.SetActive(true);
                GetDimensionBackground(ruptureDimension).gameObject.SetActive(true);

                SetTileMapMask(ruptureTilemap, SpriteMaskInteraction.VisibleInsideMask);
                GetDimensionBackground(ruptureDimension).maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            try
            {
                GetComponent<SpriteMask>().sprite = GetComponent<SpriteRenderer>().sprite;
            }
            catch
            {
                Debug.LogError("Failed to update sprite mask to current sprite");
            }
        }
    }

    void SetTileMapMask(Grid tileGrid,SpriteMaskInteraction maskMode)
    {
        foreach (TilemapRenderer tm in tileGrid.GetComponentsInChildren<TilemapRenderer>())
        {
            tm.maskInteraction = maskMode;
        }
    }

    public void Swap()
    {
        if (!closed)
        {
            GetComponent<ParticleSystem>().Play();
            if (currentDimension == mainDimension)
            {
                currentDimension = ruptureDimension;
            }
            else if (currentDimension == ruptureDimension)
            {
                currentDimension = mainDimension;
            }
            onSwap.Invoke();
        }

    }

    public IEnumerator Close()
    {
        if (!closed)
        {
            AudioManager.PlaySound("rupture",Random.Range(2.5f,3),0.4f);
            closed = true;
            count--;
            CancelInvoke();

            if(closingFx != null)
            {
                closingFx.SetActive(true);
            }

            Vector3 targetScale = closedScale;

            yield return new WaitForSeconds(0.5f);

            while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * closeSpeed);

                yield return new WaitForEndOfFrame();
            }
            transform.localScale = targetScale;

            yield return new WaitForSeconds(1);
            Destroy(gameObject);
        }
    }

    public IEnumerator Open()
    {
        if (closed)
        {
            closed = false;
            count++;
            CancelInvoke();

            if (closingFx != null)
            {
                closingFx.SetActive(true);
            }

            Vector3 targetScale = openScale;

            yield return new WaitForSeconds(0.5f);

            while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * closeSpeed);

                yield return new WaitForEndOfFrame();
            }
            transform.localScale = targetScale;

            yield return new WaitForSeconds(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WhateverSeal whatever) && !closed)//if collide with the whatever seal
        {
            StartCoroutine(Close());
        }
    }
}
