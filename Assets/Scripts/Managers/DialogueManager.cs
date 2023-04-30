using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable][Tooltip("The side the dialogue will appear on")]
public enum DialogueSides
{
    Left = 0,
    Right = 1,
        
}
[Serializable]
public struct Participant
{
    public string name;
    [Tooltip("0 = Left, 1 = Right")]
    public DialogueSides dialogueSide;
    public Sprite portrait;
}

[Serializable][Tooltip("A line of Dialogue made up of:" +
                       "\nA string line that is delivered" +
                       "\nThe participant saying the line.")]
public struct DialogueLine
{
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
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
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    //TODO Expand dialogues to include event specific dialogues: e.g. Returning/Ferrying/Start/End
    public List<DialogueStruct> Dialogues;
    public DialogueStruct currentDialogue;
    public int currentDialogueIndex;
    public int currentDialogueLine;
    public bool isDialogueActive;
    public Color inactiveSpeakerColor;
    public float inactiveSpeakerSize;

    public KeyCode nextLine;

    public static event Action OnDialogueStart; 
    public static event Action OnDialogueEnd; 

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }
    
    private void OnEnable()
    {
        GameStateManager.OnFerryingEnter += StartDialogue;
        GameStateManager.OnReturningEnter += StartDialogue;
    }

    private void OnDisable()
    {
        GameStateManager.OnFerryingEnter -= StartDialogue;
        GameStateManager.OnReturningEnter -= StartDialogue;
    }

    public void StartDialogue()
    {
        //Dont run dialogue if the game was just paused.
        if(GameStateManager.Instance.PreviousState == GameStateManager.GameStates.Pause) return;
        
        //Get the current dialogue
        currentDialogue = Dialogues[currentDialogueIndex];
        currentDialogueLine = 0;

        //Show UI/Update Text
        OnDialogueStart?.Invoke();
        isDialogueActive = true;
    }

    public List<Participant> GetCurrentParticipants()
    {
        return currentDialogue.participants;
    }

    public DialogueLine GetCurrentLine()
    {
        return (currentDialogue.linesOfDialogue[currentDialogueLine]);
    }
    public DialogueLine NextLine()
    {
        //Increment currentDialogueLine
        currentDialogueLine++;
        //Has it passed the last line?
        if (currentDialogueLine >= currentDialogue.linesOfDialogue.Count)
        {
            //Yes, dialogue exhausted. End Dialogue.
            EndDialogue();
            return new DialogueLine();
        }
        //return the current line
        return GetCurrentLine();
    }

    public void EndDialogue()
    {
        currentDialogueIndex++;
        //Has we exhausted all lines of dialogue?
        if (currentDialogueIndex >= Dialogues.Count)
        {
            //TODO Create logic for reusing dialogue.
            //TEMP** Reset to first dialogue.
            currentDialogueIndex = 0;
        }
        //Hide UI
        OnDialogueEnd?.Invoke();
        isDialogueActive = false;

    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
