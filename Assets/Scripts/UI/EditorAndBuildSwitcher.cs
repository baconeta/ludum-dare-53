using UI.StateSwitcher;
using UnityEngine;

namespace UI
{
    public class EditorAndBuildSwitcher : MonoBehaviour
    {
        // This class can be added to any object where an editor/build difference should occur in the UI, for example, 
        // in a UI with test buttons or elements that you want to disable on build but run in game in the editor.

        [SerializeField] private CompositeStateSwitcher compositeStateSwitcher;

        private void Awake()
        {
            if (!compositeStateSwitcher) Debug.LogError("No State Switcher setup on " + name);

#if UNITY_EDITOR
            compositeStateSwitcher.ChangeState("Editor");
#else
            compositeStateSwitcher.ChangeState("Build");
#endif
        }
    }
}