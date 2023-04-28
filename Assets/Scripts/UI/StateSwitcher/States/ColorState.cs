using UnityEngine.UI;

namespace UI.StateSwitcher
{
    public class ColorState : StateBase<ColorData, Graphic>
    {
        public override void Apply(ColorData value, Graphic target)
        {
            target.color = value.color;
        }
    }
}