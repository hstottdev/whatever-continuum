using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MenuState
{
    pressAny,
    main,
    levelSelect,
    options,
    credits
}

public class MenuManager : MonoBehaviour
{
    public static MenuState currentState;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] MenuSection pressAnyButton;
    [SerializeField] MenuSection main;
    [SerializeField] MenuSection levelSelect;

    [SerializeField] MenuSection options;

    [SerializeField] MenuSection credits;
    MenuSection currentSection;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.SetMusic(menuMusic);
        InstaHideAll();
        ShowCurrentMenu();
    }

    public void InstaHideAll()
    {
        pressAnyButton.InstaHide.Invoke();
        main.InstaHide.Invoke();
        levelSelect.InstaHide.Invoke();
        options.InstaHide.Invoke();
    }

    void SetCurrentMenu()
    {
        switch(currentState)
        {
            case MenuState.pressAny:
                currentSection = pressAnyButton;
                break;
            case MenuState.main:
                currentSection = main;
                break;
            case MenuState.levelSelect:
                currentSection = levelSelect;
                break;
            case MenuState.options:
                currentSection = options;
                break;
        }
    }

    void ShowCurrentMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SetCurrentMenu();
        currentSection.ShowEvent.Invoke();//show current section
        currentSection.active = true;
    }

    public void ChangeState(MenuState newState)
    {
        SetCurrentMenu();//ensure 'currentSection' variable is defined

        currentSection.HideEvent.Invoke();
        currentSection.active = false;

        currentState = newState;
        ShowCurrentMenu();
    }

    public void ChangeState(MenuSection newSection)
    {
        SetCurrentMenu();//ensure 'currentSection' variable is defined
        currentSection.HideEvent.Invoke();
        currentSection.active = false;

        currentState = newSection.state;
        ShowCurrentMenu();
    }

    private void Update() 
    {
        if(Input.GetButtonDown("Cancel"))
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        if(currentSection.parentSection != null)
        {
            ChangeState(currentSection.parentSection);
        }
    }

    public void StartLevel(string scene)
    {

    }

}
