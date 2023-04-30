using UnityEngine;
using UnityEngine.Events;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(CustomEditor.InspectorButton))]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Get MonoBehaviour script
            CustomEditor.InspectorButton inspectorButton = (CustomEditor.InspectorButton) target;

            if (GUILayout.Button(inspectorButton.buttonLabel))
            {
                // Invoke the onClickEvent event
                inspectorButton.onClickEvent.Invoke();
            }
        }
    }


}