using UnityEngine;

namespace Audio
{
    public class BackToMenuAudio : MonoBehaviour
    {
        public void StopGameAudio()
        {
            AudioWrapper.Instance.StopSound("game-background-music");
            AudioWrapper.Instance.StopSound("ambient-water");
        }
    }
}