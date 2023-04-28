using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.StateSwitcher
{
    public interface IStateContainer
    {
        void ChangeState(string newStateName);
        List<string> GetStateNames();
    }

    /// <summary>
    /// Serialized data container
    /// </summary>
    /// <typeparam name="TData">, IStateData which also will be serialized </typeparam>
    /// <typeparam name="TState"> State change behaviour </typeparam>
    /// <typeparam name="TTarget"> Target to modify </typeparam>
    [Serializable]
    public abstract class StateContainer<TData, TState, TTarget> : IStateContainer where TData : IStateData
        where TState : StateBase<TData, TTarget>, new()
    {
        public TTarget target;
        public StatesData[] states;

        private TState _state = new();

        public virtual void ChangeState(string newStateName)
        {
            foreach (StatesData stateData in states)
            {
                if (newStateName.Equals(stateData.stateName))
                {
                    _state.Apply(stateData.stateData, target);
                }
            }
        }

        public List<string> GetStateNames()
        {
            var stateNames = states.Select(data => data.stateName).Distinct().ToList();
            return stateNames;
        }

        [Serializable]
        public class StatesData
        {
            public string stateName;
            public TData stateData;
        }
    }
}