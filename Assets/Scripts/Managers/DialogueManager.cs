using System;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

[Serializable]
[Tooltip("The side the dialogue will appear on")]
public enum DialogueSides
{
    Left = 0,
    Right = 1,
}

[Serializable]
public struct Participant
{
    public string name;
    [Tooltip("0 = Left, 1 = Right")] public DialogueSides dialogueSide;
    public Sprite portrait;
}

[Serializable]
[Tooltip("A line of Dialogue made up of:" +
         "\nA string line that is delivered" +
         "\nThe participant saying the line.")]
public struct DialogueLine
{
    public bool IsEqual(DialogueLine lineToTestAgainst)
    {
        return line == lineToTestAgainst.line &&
               lineToTestAgainst.dialogueSide == dialogueSide &&
               lineToTestAgainst.participantSpeaking == participantSpeaking;
    }

    public string line;
    public DialogueSides dialogueSide;

    [Tooltip("-1 = All on side" +
             "\n0 = Left-most Participant" +
             "\n1 = 2nd, 2 = 3rd, etc...")]
    public int participantSpeaking;
}

[Serializable]
public struct DialogueStruct
{
    public List<DialogueLine> linesOfDialogue;

    [Tooltip("Element 0 is the left portrait. Element 1 is the right portrait.")]
    public List<Participant> participants;
}

[Serializable]
public struct DialogueGroup
{
    [Tooltip("DialogueStart is ran on Game Start")]
    public DialogueStruct DialogueStart;

    [Tooltip("DialogueMid is only used in Element 0 of DialogueGroups (Player's very first run)" +
             "/n DialogueMid is ran on First Ferry")]
    public DialogueStruct DialogueMid;

    [Tooltip("DialogueStart is ran on Game End")]
    public DialogueStruct DialogueEnd;
}

public class DialogueManager : EverlastingSingleton<DialogueManager>
{
    [Tooltip("Groups of dialogue that are selected at game start." +
             "\nElement 0 is played on the Player's very first run" +
             "\nOther elements are selected at random")]
    public List<DialogueGroup> DialogueGroups;

    private DialogueStruct DialogueStart;
    private DialogueStruct DialogueMid;
    private DialogueStruct DialogueEnd;
    private DialogueStruct currentDialogue = new DialogueStruct();
    public int currentDialogueLine;
    public bool isDialogueActive;
    public Color inactiveSpeakerColor;
    public float inactiveSpeakerSize;

    public bool overrideStoryElement;
    public int element;

    public KeyCode nextLine;
    private DialogueUI _dialogueUI;

    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;

    private void Awake()
    {
        base.Awake();
        _dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void OnEnable()
    {
        GameStateManager.OnStartEnter += StartDialogue;
        //I think this is never used?
        //GameStateManager.OnFerryingEnter += StartDialogue;
        GameStateManager.OnReturningEnter += StartDialogue;
        GameStateManager.OnEndEnter += StartDialogue;

        InputManager.onDialogueNext += NextLineAction;
    }

    private void OnDisable()
    {
        GameStateManager.OnStartEnter -= StartDialogue;
        //GameStateManager.OnFerryingEnter -= StartDialogue;
        GameStateManager.OnReturningEnter -= StartDialogue;
        GameStateManager.OnEndEnter -= StartDialogue;
    }

    private void Start()
    {
        if (!_dialogueUI) _dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void SelectDialogueGroup()
    {
        if (!_dialogueUI) _dialogueUI = FindObjectOfType<DialogueUI>();
        DialogueGroup newDialogueGroup;

        //If its their first play-through, choose the first DialogueGroup
        if (PlayerPrefs.GetInt("Successful Ferries") == 0)
        {
            newDialogueGroup = DialogueGroups[0];
        }
        else
        {
            // Get a random dialogue group (Excluding element 0). 
            if (overrideStoryElement)
            {
                newDialogueGroup = DialogueGroups[element];
            }
            else
            {
                newDialogueGroup = DialogueGroups[Random.Range(1, DialogueGroups.Count)];
            }
        }

        DialogueStart = newDialogueGroup.DialogueStart;
        DialogueMid = newDialogueGroup.DialogueMid;
        DialogueEnd = newDialogueGroup.DialogueEnd;
    }

    public void StartDialogue()
    {
        GameStateManager.GameStates previousState = GameStateManager.Instance.PreviousState;
        GameStateManager.GameStates currentState = GameStateManager.Instance.CurrentState;

        //On Start, Get a new Dialogue Group
        if (currentState == GameStateManager.GameStates.Start)
            SelectDialogueGroup();

        //Clear current dialogue.
        currentDialogue = new DialogueStruct();
        currentDialogueLine = 0;

        //If the game is not ending
        if (currentState != GameStateManager.GameStates.End)
        {
            switch (previousState)
            {
                //Game just started
                case GameStateManager.GameStates.Start:
                    if (currentState == GameStateManager.GameStates.Ferrying) return;
                    //Play intro dialogue
                    currentDialogue = DialogueStart;
                    break;

                //Ferried souls successfully
                case GameStateManager.GameStates.Ferrying:
                    if (currentState != GameStateManager.GameStates.Returning) break;
                    //Only occurs on the first play through
                    if (PlayerPrefs.GetInt("Successful Ferries") > 0) return;
                    //Only occurs once on first successful ferry
                    if (GameStateManager.Instance.firstFerryCompleted) return;
                    //Play mid-game dialogue
                    if (DialogueMid.linesOfDialogue.Count > 0)
                    {
                        currentDialogue = DialogueMid;
                        break;
                    }
                    return;
            }
        }
        else
        {
            //Play ending dialogue.
            currentDialogue = DialogueEnd;
        }

        //If a dialogue wasn't selected/IsEmpty, abort StartDialogue.
        if (currentDialogue.linesOfDialogue is null) return;

        isDialogueActive = true;
        //Show UI/Update Text
        OnDialogueStart?.Invoke();
    }

    public List<Participant> GetCurrentParticipants()
    {
        return currentDialogue.participants;
    }

    public DialogueLine GetCurrentLine()
    {
        return (currentDialogue.linesOfDialogue[currentDialogueLine]);
    }

    public void NextLineAction()
    {
        NextLine();
    }

    public void NextLine()
    {
        //If the line doesnt exist, dont do it.
        if (currentDialogue.linesOfDialogue == null) return;

        //Increment currentDialogueLine
        currentDialogueLine++;

        //Has it passed the last line?
        if (currentDialogueLine >= currentDialogue.linesOfDialogue.Count)
        {
            //Yes, dialogue exhausted. End Dialogue.
            EndDialogue();
            return;
        }

        _dialogueUI.UpdateDialogueScene(GetCurrentLine());
    }

    public void EndDialogue()
    {
        PlayerPrefs.SetInt("TutorialSeen", 1); // We always assume since we show the tutorial first, that it has been seen
        isDialogueActive = false;
        //Hide UI
        OnDialogueEnd?.Invoke();
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}