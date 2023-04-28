using UnityEngine.UI;

namespace UI.StateSwitcher
{
    public class SpriteState : StateBase<SpriteData, Image>
    {
        public override void Apply(SpriteData value, Image target)
        {
            target.sprite = value.sprite;
        }
    }
}