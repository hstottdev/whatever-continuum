using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessShuffler : MonoBehaviour
{
    [SerializeField] PostProcessVolume targetVolume;
    PostProcessProfile originalProfile;

    bool shuffling;
    [SerializeField] bool playOnAwake;
    public float swapInterval;
    [SerializeField] List<PostProcessProfile> profileList;
    [SerializeField] PostProcessProfile profileToEndOn;
    // Start is called before the first frame update
    void Awake()
    {
        if (playOnAwake)
        {
            StartShuffling();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwapToRandomProfile()
    {      
        if(shuffling)
        {
            targetVolume.profile = profileList[Random.Range(0, profileList.Count)];

            Invoke("SwapToRandomProfile", swapInterval);
        }
    }

    public void StartShuffling()
    {
        shuffling = true;
        SwapToRandomProfile();      
    }

    public void StopShuffling()
    {
        shuffling = false;
        if(profileToEndOn != null)
        {
            targetVolume.profile = profileToEndOn;
        }
    }

}
