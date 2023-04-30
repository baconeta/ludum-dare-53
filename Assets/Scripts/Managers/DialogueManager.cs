using System;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    [Serializable]
    public struct DialogueStruct {
        public List<string> linesOfDialogue;
    }

    public class DialogueManager : MonoBehaviour {
        public static DialogueManager instance;

        //TODO Expand dialogues to include event specific dialogues: e.g. Returning/Ferrying/Start/End
        public List<DialogueStruct> Dialogues;
        public DialogueStruct currentDialogue;
        public int currentDialogueIndex;
        public int currentDialogueLine;
        public bool isDialogueActive;

        public KeyCode nextLine;

        public static event Action OnDialogueStart;
        public static event Action OnDialogueEnd;

        private void Awake() {
            if (!instance) instance = this;
            else Destroy(this);
        }

        private void OnEnable() {
            GameStateManager.OnFerryingEnter += StartDialogue;
            GameStateManager.OnReturningEnter += StartDialogue;
        }

        private void OnDisable() {
            GameStateManager.OnFerryingEnter -= StartDialogue;
            GameStateManager.OnReturningEnter -= StartDialogue;
        }

        public void StartDialogue() {
            //Dont run dialogue if the game was just paused.
            if (GameStateManager.Instance.PreviousState == GameStateManager.GameStates.Pause) return;

            //Get the current dialogue
            currentDialogue = Dialogues[currentDialogueIndex];
            currentDialogueLine = 0;


            //Show UI/Update Text
            OnDialogueStart?.Invoke();
            isDialogueActive = true;
        }

        public string GetCurrentLine() {
            return (currentDialogue.linesOfDialogue[currentDialogueLine]);
        }

        public string NextLine() {
            //Increment currentDialogueLine
            currentDialogueLine++;
            //Has it passed the last line?
            if (currentDialogueLine >= currentDialogue.linesOfDialogue.Count)
            {
                //Yes, dialogue exhausted. End Dialogue.
                EndDialogue();
                return "";
            }

            //return the current line
            return GetCurrentLine();
        }

        public void EndDialogue() {
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

        public bool IsDialogueActive() {
            return isDialogueActive;
        }
    }
}
