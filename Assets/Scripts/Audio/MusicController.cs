using UnityEngine;

namespace Audio
{
    public class MusicController : MonoBehaviour
    {
        private void Start()
        {
#if !UNITY_EDITOR
        AudioWrapper.Instance.PlaySound("game-background-music");
#endif
        }
    }
}