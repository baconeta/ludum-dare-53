using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DialogueStruct
{
    public List<string> linesOfDialogue;
}
public class DialogueManager : MonoBehaviour
{
    public List<DialogueStruct> Dialogues;
    public DialogueStruct currentDialogue;
    public int currentDialogue;

    public int currentLine = 0;
    
    public KeyCode nextLine;

    void StartDialogue()
    {
        currentLine = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(nextLine))
        {
            
        }
    }
}
