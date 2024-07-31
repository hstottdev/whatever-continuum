using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] Button startingButton;
    public static bool paused;
    [SerializeField] GameObject pauseUI;
    // Start is called before the first frame update
    void Start()
    {
        if(startingButton != null)
        {
            startingButton.Select();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (Input.GetButtonDown("Pause") && pauseUI != null && !LevelManager.lost && !LevelManager.won && !LevelManager.disableInputs)
        {
            if (paused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        InputChecker.startButton = pauseUI.GetComponentInChildren<Button>();
        InputChecker.SelectFirstActiveButton();
        paused = true;
        //Time.timeScale = 0;
        AudioManager.inst.music.Pause();
        AudioManager.PlaySound("click1",0.8f);
    }

    public void UnPause()
    {
        InputChecker.DeselectActiveButton();
        pauseUI.SetActive(false);
        paused = false;
        Time.timeScale = 1;
        AudioManager.inst.music.Play();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if(paused)
        {
            UnPause();
        }
    }

    public void StartLevelOne()
    {
        if(AudioManager.inst != null)
        {
            AudioManager.inst.music.time = 0;
        }
        LevelManager.mode = LevelMode.regular;
        SceneManager.LoadScene("Level 1");
    }

    public void StartLevelZero()
    {
        if (AudioManager.inst != null)
        {
            AudioManager.inst.music.time = 0;
        }
        LevelManager.mode = LevelMode.regular;
        SceneManager.LoadScene("Tutorial Level");
    }

    public void RestartLevel()
    {
        LevelManager.mode = LevelMode.regular;
        Retry();
    }

    public void MainMenu()
    {
        if(AudioManager.inst != null)
        {
            AudioManager.inst.music.time = 0;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
