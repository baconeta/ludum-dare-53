using UnityEngine;

namespace UI.StateSwitcher
{
    public abstract class StateBaseUnityComponent : MonoBehaviour
    {
        public abstract void ChangeState(string newState);
        public abstract IStateContainer[] StateContainers { get; }
    }
}