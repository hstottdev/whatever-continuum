using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum LevelMode
{
    regular,
    whatever,
    finished
}

public class LevelManager : MonoBehaviour
{
    public static LevelMode mode;
    public static int minorFound;
    public static int majorFound;
    public static int totalMinor;
    public static int totalMajor;
    public static float sealTime;
    public UnityEvent onWin;
    [SerializeField] AudioClip lossSong;
    [SerializeField] timer sealTimer;
    public static bool won;
    public static bool lost;
    public static bool disableInputs;

    private void Awake()
    {
        totalMajor = 0;
        totalMinor = 0;
        RuptureManager.count = 0;
    }

    private void Start()
    {
        Time.timeScale = 1;
        ButtonManager.paused = false;
        lost = false;
        won = false;

        if(mode == LevelMode.whatever)
        {
            Invoke("StartWhateverMode", 0.01f);
        }
        else
        {
            mode = LevelMode.regular;
            minorFound = 0;
            majorFound = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        sealTimer.enabled = !ButtonManager.paused;

        if (IsCompleted() && !ButtonManager.paused)
        {
            Debug.Log("FINISHED!!!");
            mode = LevelMode.finished;
            if(PlayerController.instance.GetComponent<WhateverSeal>() != null)
            {
                WhateverSeal seal = PlayerController.instance.GetComponent<WhateverSeal>();
                seal.StartCoroutine(seal.EnterVortex());
            }
            onWin.Invoke();
            sealTime = sealTimer.startValue - sealTimer.time;
            won = true;

        }

        if(sealTimer.time <= 0 && !won && !ButtonManager.paused)
        {
            if(!lost)
            {
                lost = true;
                UiManager.instance.Invoke("ShowLossScreen", 1f);
            }

            if (!AudioManager.inst.music.isPlaying)
            {
                AudioManager.SetMusic(lossSong);
                AudioManager.inst.music.loop = true;
            }

            if (UiManager.instance.loseScreen.cg.alpha >= 0.5f && UiManager.instance.loseScreen.cg.interactable == false)
            {
                UiManager.instance.loseScreen.cg.interactable = true;
            }
        }
    }

    public void SetMusic(AudioClip audioClip)
    {
        AudioManager.SetMusic(audioClip);
    }

    public void StartWhateverMode()
    {
        if(PlayerController.instance.GetComponent<WhateverSeal>() == null)
        {
            PlayerController.instance.ToggleSpriteAndCollider(false, true);//if normal player then hide me
        }
        Vector2 gatewayPosition = FindObjectOfType<WhateverDoor>().transform.position;
        PlayerController.instance.transform.position = gatewayPosition;
    }

    public void TogglePlayerInput(bool toggle)
    {
        disableInputs = !toggle;
    }

    bool IsCompleted()
    {
        return mode == LevelMode.whatever && RuptureManager.count == 0 && !won;
    }
}
