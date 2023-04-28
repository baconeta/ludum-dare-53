using UnityEngine;

namespace UI.StateSwitcher
{
    public abstract class SpecificStateComponentBase<TStateContainer> : StateBaseUnityComponent
        where TStateContainer : IStateContainer
    {
        [SerializeField] private TStateContainer[] stateContainers;
        public override IStateContainer[] StateContainers => stateContainers as IStateContainer[];

        public override void ChangeState(string newState)
        {
            foreach (TStateContainer stateContainer in stateContainers)
            {
                stateContainer.ChangeState(newState);
            }
        }
    }
}