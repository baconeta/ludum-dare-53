using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.StateSwitcher
{
    public class CompositeStateSwitcher : MonoBehaviour
    {
        [SerializeField] private StateBaseUnityComponent[] stateChangers;

        [HideInInspector] public int index;
        private List<string> _statesCache = new();

        public List<string> States
        {
            get
            {
                if (_statesCache.Any())
                {
                    return _statesCache;
                }

                _statesCache = GetStates();
                return _statesCache;
            }
        }

        private List<string> GetStates()
        {
            if (stateChangers is not {Length: > 0})
            {
                return new List<string>();
            }

            var stateNames = new List<string>();

            foreach (StateBaseUnityComponent stateChanger in stateChangers)
            {
                if (stateChanger == null || stateChanger.StateContainers == null)
                {
                    continue;
                }

                foreach (IStateContainer stateContainer in stateChanger.StateContainers)
                {
                    stateNames.AddRange(stateContainer.GetStateNames());
                }
            }

            return stateNames.Distinct().ToList();
        }

        public void UpdateStates()
        {
            _statesCache = GetStates();
        }

        public void TestState()
        {
            ChangeState(_statesCache[index]);
        }

        public void ChangeState(string state)
        {
            foreach (StateBaseUnityComponent stateChanger in stateChangers)
            {
                stateChanger.ChangeState(state);
            }
        }
    }
}