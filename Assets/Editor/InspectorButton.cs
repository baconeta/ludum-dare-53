using UnityEngine;
using UnityEngine.Events;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(InspectorButton))]
    public class InspectorButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Get MonoBehaviour script
            InspectorButton inspectorButton = (InspectorButton) target;

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
}