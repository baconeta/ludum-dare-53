using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[UnityEditor.CustomEditor(typeof(InspectorButton))]
public class InspectorButtonEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //Get MonoBehaviour script
        InspectorButton inspectorButton = (InspectorButton)target;
        
        if (GUILayout.Button(inspectorButton.buttonLabel))
        {
            // Invoke the onClickEvent event
            inspectorButton.onClickEvent.Invoke();
        }
    }
}

public class InspectorButton : MonoBehaviour
{
    public string buttonLabel = "Button";
    public UnityEvent onClickEvent;
}
