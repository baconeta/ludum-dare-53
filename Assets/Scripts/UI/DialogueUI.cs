using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueTextRight;
    [SerializeField] private TextMeshProUGUI dialogueNameRight;
    [SerializeField] private TextMeshProUGUI dialogueTextLeft;
    [SerializeField] private TextMeshProUGUI dialogueNameLeft;
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
        var count = 0;
        while (leftSideParticipants.Count > 0)
        {
            count += 1;
            GameObject participantGO = leftSideParticipants[0];
            if (participantGO is not null)
            {
                leftSideParticipants.Remove(participantGO);
                Destroy(participantGO);
            }

            if (count >= 10) break;
        }

        count = 0;
        while (rightSideParticipants.Count > 0)
        {
            count += 1;
            GameObject participantGO = rightSideParticipants[0];
            if (participantGO is not null)
            {
                rightSideParticipants.Remove(participantGO);
                Destroy(participantGO);
            }
            if (count >= 10) break;
        }
    }

    public void UpdateDialogueScene(DialogueLine nextLine)
    {
        GameObject activeParticipant = null;
        nextLine.line = nextLine.line.Replace("{{TOTAL_SOULS}}", PlayerPrefs.GetInt("TotalSoulsSaved", 0).ToString());

        //Single speaker cases
        if (nextLine.participantSpeaking >= 0)
        {
            //Update the current active speaker
            switch (nextLine.dialogueSide)
            {
                //Left side, get the current speaker.
                case DialogueSides.Left:
                    if (nextLine.participantSpeaking >= leftSideParticipants.Count) break;
                    activeParticipant = leftSideParticipants[nextLine.participantSpeaking];
                    if (activeParticipant is not null)
                    {
                        rightDialoguePanel.SetActive(false);
                        leftDialoguePanel.SetActive(true);
                        //Set color to white (The original Colour)
                        activeParticipant.GetComponent<Image>().color = Color.white;
                        //Set active participant size to 1,1,1
                        activeParticipant.GetComponent<RectTransform>().localScale = Vector3.one;
                        //Update the current text with the Syntax "~"Name: Text"
                        dialogueNameRight.text = activeParticipant.name.ToUpper() + ":";
                        dialogueTextRight.text = nextLine.line;
                    }
                    else dialogueTextRight.text = nextLine.line;

                    break;
                case DialogueSides.Right:
                    if (nextLine.participantSpeaking >= rightSideParticipants.Count) break;
                    activeParticipant = rightSideParticipants[nextLine.participantSpeaking];
                    if (activeParticipant is not null)
                    {
                        rightDialoguePanel.SetActive(true);
                        leftDialoguePanel.SetActive(false);
                        //Set color to white (The original Colour)
                        activeParticipant.GetComponent<Image>().color = Color.white;
                        //Set active participant size to 1,1,1
                        activeParticipant.GetComponent<RectTransform>().localScale = Vector3.one;
                        //Update the current text with the Syntax "~"Name: Text"
                        dialogueNameLeft.text = activeParticipant.name.ToUpper() + ":";
                        dialogueTextLeft.text = nextLine.line;
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
        //No speaker case - Tutorial special case
        else if (nextLine.participantSpeaking == -2)
        {
            if (CheckIfTutorialPlayed()) // Skip the tutorial if it has been completed already
            {
                var dm = FindObjectOfType<DialogueManager>();
                if (dm != null) dm.NextLineAction();
                return;
            }
            
            rightDialoguePanel.SetActive(false);
            leftDialoguePanel.SetActive(true);
            //Update the current active speaker
            switch (nextLine.dialogueSide)
            {
                case DialogueSides.Left:
                    dialogueTextRight.text = nextLine.line.Replace("\\n", "\n");
                    break;
                case DialogueSides.Right:
                    dialogueTextLeft.text = nextLine.line.Replace("\\n", "\n");
                    break;
            }


            //Deactivate all leftSideParticipants except activeParticipant
            foreach (GameObject participant in leftSideParticipants)
            {
                //Inactive participants
                //Set colour to inactive
                participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor / 2;
                //Set size to inactive size
                participant.GetComponent<RectTransform>().localScale =
                    Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
            }

            //Deactivate all rightSideParticipants except activeParticipant
            foreach (GameObject participant in rightSideParticipants)
            {
                //Inactive participants
                //Set colour to inactive
                participant.GetComponent<Image>().color = DialogueManager.Instance.inactiveSpeakerColor / 2;
                //Set size to inactive size
                participant.GetComponent<RectTransform>().localScale =
                    Vector3.one * DialogueManager.Instance.inactiveSpeakerSize;
            }
        }
        else //All of one side speaking case
        { // Disabled for bug fixing purposes
            return;
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
            
            if (nextLine.participantSpeaking != -2)
            {
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

    private bool CheckIfTutorialPlayed()
    {
        return PlayerPrefs.GetInt("TutorialSeen", 0) == 1;
    }
}