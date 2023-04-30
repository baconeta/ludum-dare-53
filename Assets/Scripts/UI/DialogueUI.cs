using System;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void OnEnable()
    {
        DialogueManager.OnDialogueStart += ShowDialogue;
        DialogueManager.OnDialogueEnd += HideDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueStart -= ShowDialogue;
        DialogueManager.OnDialogueEnd -= HideDialogue;

    }

    private void Start()
    {
        ui.SetActive(false);
    }

    private void ShowDialogue()
    {
        ui.SetActive(true);
        dialogueText.text = DialogueManager.instance.GetCurrentLine();
    }

    private void HideDialogue()
    {
        ui.SetActive(false);
    }

    /// <summary>
    /// Pause / Resume game when Escape is pressed
    /// </summary>
    void OnGUI()
    {
        //Don't process if UI is not active
        if (!ui.activeSelf) return;
        
        //TODO Add Any Key to continue dialogue
        Event e = Event.current;
        if (!e.isKey || e.type != EventType.KeyUp || e.keyCode != DialogueManager.instance.nextLine) return;

        //Update text with the new line
        string nextLine = DialogueManager.instance.NextLine();
        
        //If the dialogue is exhausted, close dialogue.
        if(nextLine == "") HideDialogue();
        else dialogueText.text = nextLine;
            




    }
}