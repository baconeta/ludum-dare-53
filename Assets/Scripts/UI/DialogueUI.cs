using System;
using Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueTextRight;
    [SerializeField] private TextMeshProUGUI dialogueTextLeft;
    [SerializeField] private GameObject leftDialoguePanel;
    [SerializeField] private GameObject rightDialoguePanel;
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
        foreach (Participant participant in DialogueManager.Instance.GetCurrentParticipants())
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
        UpdateDialogueScene(DialogueManager.Instance.GetCurrentLine());
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

    public void UpdateDialogueScene(DialogueLine nextLine)
    {
        GameObject activeParticipant = null;

        //Single speaker cases
        if (nextLine.participantSpeaking >= 0)
        {
            //Update the current active speaker
            switch (nextLine.dialogueSide)
            {
                //Left side, get the current speaker.
                case DialogueSides.Left:
                    rightDialoguePanel.SetActive(false);
                    leftDialoguePanel.SetActive(true);
                    if (nextLine.participantSpeaking >= leftSideParticipants.Count) break;
                    activeParticipant = leftSideParticipants[nextLine.participantSpeaking];
                    if (activeParticipant is not null)
                    {
                        //Set color to white (The original Colour)
                        activeParticipant.GetComponent<Image>().color = Color.white;
                        //Set active participant size to 1,1,1
                        activeParticipant.GetComponent<RectTransform>().localScale = Vector3.one;
                        //Update the current text with the Syntax "~"Name: Text"
                        dialogueTextRight.text = activeParticipant.name + ": " + nextLine.line;
                    }
                    else dialogueTextRight.text = nextLine.line;

                    break;
                case DialogueSides.Right:
                    rightDialoguePanel.SetActive(true);
                    leftDialoguePanel.SetActive(false);
                    if (nextLine.participantSpeaking >= rightSideParticipants.Count) break;
                    activeParticipant = rightSideParticipants[nextLine.participantSpeaking];
                    if (activeParticipant is not null)
                    {
                        //Set color to white (The original Colour)
                        activeParticipant.GetComponent<Image>().color = Color.white;
                        //Set active participant size to 1,1,1
                        activeParticipant.GetComponent<RectTransform>().localScale = Vector3.one;
                        //Update the current text with the Syntax "~"Name: Text"
                        dialogueTextLeft.text = activeParticipant.name + ": " + nextLine.line;
                    }
                    else dialogueTextLeft.text = nextLine.line;

                    break;
            }


            //Deactivate all leftSideParticipants except activeParticipant
            foreach (GameObject participant in leftSideParticipants)
            {
                //Check against active participant, don't alter if it is.
                if (participant == activeParticipant) continue;

                //Inactive participants
                //Set colour to inactive
                participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor;
                //Set size to inactive size
                participant.GetComponent<RectTransform>().localScale =
                    Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
            }

            //Deactivate all rightSideParticipants except activeParticipant
            foreach (GameObject participant in rightSideParticipants)
            {
                //Check against active participant, don't alter if it is.
                if (participant == activeParticipant) continue;

                //Inactive participants
                //Set colour to inactive
                participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor;
                //Set size to inactive size
                participant.GetComponent<RectTransform>().localScale =
                    Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
            }
        }
        else //All of one side speaking case
        {
            List<GameObject> activeParticipants = new List<GameObject>();

            //Update the current active speaker
            switch (nextLine.dialogueSide)
            {
                //Left side, get the current speaker.
                case DialogueSides.Left:
                    activeParticipants = leftSideParticipants;
                    //Active all leftSideParticipants
                    foreach (GameObject participant in leftSideParticipants)
                    {
                        //Set colour to Active
                        participant.GetComponent<Image>().color = Color.white;
                        //Set size to inactive size
                        participant.GetComponent<RectTransform>().localScale =
                            Vector3.one;
                    }

                    //Deactive all rightSideParticipants
                    foreach (GameObject participant in rightSideParticipants)
                    {
                        //Set colour to inactive
                        participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor;
                        //Set size to inactive size
                        participant.GetComponent<RectTransform>().localScale =
                            Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
                    }

                    break;

                case DialogueSides.Right:
                    activeParticipants = rightSideParticipants;
                    //Active all rightSideParticipants
                    foreach (GameObject participant in rightSideParticipants)
                    {
                        //Set colour to Active
                        participant.GetComponent<Image>().color = Color.white;
                        //Set size to inactive size
                        participant.GetComponent<RectTransform>().localScale =
                            Vector3.one;
                    }

                    //Deactive all leftSideParticipants
                    foreach (GameObject participant in leftSideParticipants)
                    {
                        //Set colour to inactive
                        participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor;
                        //Set size to inactive size
                        participant.GetComponent<RectTransform>().localScale =
                            Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
                    }

                    break;
            }

            string names = "";
            foreach (GameObject participant in activeParticipants)
            {
                if (names == "")
                    names = participant.name;
                else
                {
                    names = names + " & " + participant.name;
                }
            }

            //Update the current text with the Syntax "Name: Text"
            switch (nextLine.dialogueSide)
            {
                case DialogueSides.Right:

                    dialogueTextLeft.text = names + ": " + nextLine.line;
                    break;
                case DialogueSides.Left:
                    dialogueTextRight.text = names + ": " + nextLine.line;
                    break;
            }
        }
    }
}