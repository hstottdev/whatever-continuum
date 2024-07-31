using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class collectableCounter : MonoBehaviour
{
    public TextMeshProUGUI sockCounter;
    public TextMeshProUGUI catCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCollectableCounters()
    {
        if(LevelManager.totalMinor > 0 && LevelManager.totalMajor > 0)
        {
            sockCounter.text = LevelManager.minorFound.ToString();

            catCounter.text = $"{LevelManager.majorFound}/{LevelManager.totalMajor}";
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
     
    private void OnEnable()
    {
        UpdateCollectableCounters();
    }
}
