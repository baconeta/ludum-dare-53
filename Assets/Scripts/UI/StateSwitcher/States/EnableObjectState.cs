using UnityEngine;

namespace UI.StateSwitcher
{
    public class EnableObjectState : StateBase<EnableObjectData, GameObject>
    {
        public override void Apply(EnableObjectData value, GameObject target)
        {
            target.SetActive(value.enable);
        }
    }
}