using System;
using UnityEngine.UI;

namespace UI.StateSwitcher
{
    [Serializable]
    public class SpriteStateContainer : StateContainer<SpriteData, SpriteState, Image>{}
}