using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [SerializeField] TextMeshProUGUI ruptureCounter;
    [SerializeField] GameObject sealUI;
    public canvasFader loseScreen;
    [SerializeField] Button retrySealButton;
    [SerializeField] Button startOverButton;
    public ParticleSystem sprintFX;

    [Header("VICTORY")]
    public Image vortexVoid;
    [SerializeField] TextMeshProUGUI sealTimeText;
    public Button continueButton;
    public GameObject winScreen;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (LevelManager.mode)
        {
            case LevelMode.regular:;
                sealUI.SetActive(false);
                retrySealButton.gameObject.SetActive(false);
                startOverButton.gameObject.SetActive(true);
                break;
            case LevelMode.whatever:
                sealUI.SetActive(true);
                retrySealButton.gameObject.SetActive(true);
                startOverButton.gameObject.SetActive(false);
                UpdateRuptureCounter();
                break;
            case LevelMode.finished:
                sealUI.SetActive(false);
                string numberText = Console.RoundToDP(LevelManager.sealTime,3).ToString();
                if (!numberText.Contains("."))
                {
                    numberText += ".000";
                }
                else if (numberText.Split('.')[1].Length == 1)
                {
                    numberText += ".00";
                }
                else if (numberText.Split('.')[1].Length == 2)
                {
                    numberText += ".0";
                }
                if (numberText.Split('.')[0].Length == 1) numberText = "0" + numberText;
                numberText += "s";
                sealTimeText.text = numberText;
                break;

        }      
    }

    public void ShowLossScreen()
    {
        loseScreen.gameObject.SetActive(true);
        loseScreen.StartFade();
        InputChecker.startButton = loseScreen.GetComponentInChildren<Button>();
        InputChecker.SelectFirstActiveButton();
    }

    void UpdateRuptureCounter()
    {
        ruptureCounter.text = RuptureManager.count.ToString();
    }
}
