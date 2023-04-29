using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[UnityEditor.CustomEditor(typeof(InspectorButton))]
public class InspectorButtonEditor : Editor
{
    public string buttonLabel = "Button";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button(buttonLabel))
        {
            // Get the target MyMonoBehaviour script
            InspectorButton inspectorButton = (InspectorButton)target;

            // Invoke the MyEvent event
            inspectorButton.onClickEvent.Invoke();
        }
    }
}

public class InspectorButton : MonoBehaviour
{
    public UnityEvent onClickEvent;

}
