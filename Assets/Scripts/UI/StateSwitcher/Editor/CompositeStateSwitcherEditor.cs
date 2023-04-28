using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UI.StateSwitcher
{
    [CustomEditor(typeof(CompositeStateSwitcher))]
    public class CompositeStateSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CompositeStateSwitcher script = (CompositeStateSwitcher) target;

            script.UpdateStates(); // you can change it to manual update if it starts to cause editor performance issues

            GUIContent arrayList = new("States");
            script.index = EditorGUILayout.Popup(arrayList, script.index, script.States.ToArray());

            if (GUILayout.Button("Test the state"))
            {
                script.TestState();
            }
        }
    }
}
#endif