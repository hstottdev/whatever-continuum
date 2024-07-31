using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameDialogue : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    [SerializeField] Image portraitImage;
    [SerializeField] Image frameImage;

    [SerializeField] List<CharacterData> characters;
    Dictionary<string, CharacterData> nameDict;
    public UnityEvent ProgressDialogue;

    public UnityEvent OnDialogueEnd;
    bool dialogueActive;
    // Start is called before the first frame update
    void Awake()
    {
        SetNameDict();
        TogglePortrait(false);
        dialogueRunner = GetComponent<DialogueRunner>();
        dialogueRunner.AddCommandHandler<string>("portrait",SetPortrait);
        dialogueRunner.AddCommandHandler<bool>("dialogue", ToggleDialogue);
    }

    void SetNameDict()
    {
        nameDict = new Dictionary<string, CharacterData>();

        foreach(CharacterData c in characters)
        {
            nameDict.Add(c.characterName,c);
        }
    }

    void SetPortrait(string character)
    {
        if(nameDict == null) SetNameDict();

        if(nameDict.ContainsKey(character))
        {
            TogglePortrait(true);
            CharacterData cData = nameDict[character];
            portraitImage.sprite = cData.portrait;
            frameImage.color = cData.frameColor;
        }
        else
        {
            TogglePortrait(false);
        }
    }

    private void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Pause"))
        {
          OnProgressDialogue();
        }         
    }

    void TogglePortrait(bool toggle)
    {
        portraitImage.enabled = toggle;
        portraitImage.transform.parent.GetComponent<Image>().enabled = toggle;
    }

    public void OnProgressDialogue()
    {
        ProgressDialogue.Invoke();
        
        if(dialogueActive) AudioManager.PlaySound("click1",1.5f,0.4f);
    }

    public void ToggleDialogue(bool toggle)
    {
        LevelManager.disableInputs = toggle;
        dialogueActive = toggle;

        if(toggle == false)
        {
            OnDialogueEnd.Invoke();
        }
    }

    public void PlaySound(string fileName)
    {
        AudioManager.PlaySound(fileName);
    }
}
