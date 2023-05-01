using UnityEngine;

namespace Audio
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private string trackToPlay;
        [SerializeField] private bool playOnStart = true;

        private void Start()
        {
// #if !UNITY_EDITOR && playOnStart
            if (playOnStart)
            {
                AudioWrapper.Instance.PlaySound(trackToPlay);
            }

// #endif
        }

        public void PlayManually()
        {
            AudioWrapper.Instance.PlaySound(trackToPlay);
        }

        public void StopTrack()
        {
            AudioWrapper.Instance.StopSound(trackToPlay);
        }
    }
}