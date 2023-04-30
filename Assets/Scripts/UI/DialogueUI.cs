using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject leftSide;
    [SerializeField] private List<GameObject> leftSideParticipants;
    [SerializeField] private GameObject rightSide;
    [SerializeField] private List<GameObject> rightSideParticipants;
    [SerializeField] private GameObject CharacterPrefab;

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
        RemoveParticipantGameObjects();
    }

    private void ShowDialogue()
    {
        ui.SetActive(true);

        //TODO Replace with object pooling /  Participant Builder 
        //Add all participants to the scene
        foreach (Participant participant in DialogueManager.instance.GetCurrentParticipants())
        {
            GameObject newParticipantGO = null;
            switch (participant.dialogueSide)
            {
                //Add Participant to left side
                case DialogueSides.Left:
                    newParticipantGO = Instantiate(CharacterPrefab, leftSide.transform);
                    leftSideParticipants.Add(newParticipantGO);
                    break;
                
                //Add Participant to Right Side
                case DialogueSides.Right:
                    newParticipantGO = Instantiate(CharacterPrefab, rightSide.transform);
                    
                    rightSideParticipants.Add(newParticipantGO);
                    break;
            }

            //This null check makes my compiler happy, newParticipantGO should never be null.
            if (newParticipantGO is not null)
            {
                //Update GameObject Name
                newParticipantGO.name = participant.name;
                //Update Image
                newParticipantGO.GetComponent<Image>().sprite = participant.portrait;
            }
        }
        //Update the dialogue scene with the current line
        UpdateDialogueScene(DialogueManager.instance.GetCurrentLine());
    }

    private void HideDialogue()
    {
        ui.SetActive(false);

        RemoveParticipantGameObjects();
    }

    //Safely removes all participants from the screen.
    private void RemoveParticipantGameObjects()
    {
        while (leftSideParticipants.Count > 0)
        {
            GameObject participantGO = leftSideParticipants[0];
            leftSideParticipants.Remove(participantGO);
            Destroy(participantGO);
        }
        while (rightSideParticipants.Count > 0)
        {
            GameObject participantGO = rightSideParticipants[0];
            rightSideParticipants.Remove(participantGO);
            Destroy(participantGO);
        }
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
        DialogueLine nextLine = DialogueManager.instance.NextLine();
        
        //If the dialogue is exhausted (Empty DialogueLine), close dialogue.
        if(nextLine.Equals(new DialogueLine())) HideDialogue();
        else //Use nextLine to update the Dialogue Scene
        {
            UpdateDialogueScene(nextLine);
        }
    }

    private void UpdateDialogueScene(DialogueLine nextLine)
    {
        GameObject activeParticipant = null;

        //Update the current active speaker
        //TODO Edge case for multiple speakers
        switch (nextLine.dialogueSide)
        {
            //Left side, get the current speaker.
            case DialogueSides.Left:
                activeParticipant = leftSideParticipants[nextLine.participantSpeaking];
                break;
            case DialogueSides.Right:
                activeParticipant = rightSideParticipants[nextLine.participantSpeaking];
                break;
        }

        //Set color to white (The original Colour)
        activeParticipant.GetComponent<Image>().color = Color.white;
        //Set active participant size to 1,1,1
        activeParticipant.GetComponent<RectTransform>().localScale = Vector3.one;


        //Update the current text with the Syntax "~"Name: Text"
        dialogueText.text = activeParticipant.name + ": " + nextLine.line;

        foreach (GameObject participant in leftSideParticipants)
        {
            //Check against active participant, don't alter if it is.
            if (participant == activeParticipant) continue;

            //Inactive participants
            //Set colour to inactive
            participant.GetComponent<Image>().color = DialogueManager.instance.inactiveSpeakerColor;
            //Set size to inactive size
            participant.GetComponent<RectTransform>().localScale =
                Vector3.one * DialogueManager.instance.inactiveSpeakerSize;
        }
    }
}