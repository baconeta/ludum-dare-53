using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utils;

namespace Audio
{
    public class AudioWrapper : EverlastingSingleton<AudioWrapper>
    {
        [SerializeField] private List<SoundData> allSoundData;
        private readonly Dictionary<string, SoundData> _soundDict = new();

        private bool _dictionaryInitialised;

        private Dictionary<string, AudioSourcePoolable> playingSounds;

        protected override void Awake()
        {
            base.Awake();
            if (_dictionaryInitialised) return;

            // Prep the dictionary 
            foreach (SoundData sound in allSoundData)
            {
                _soundDict.Add(sound.name, sound);
            }

            _dictionaryInitialised = true;
            playingSounds = new Dictionary<string, AudioSourcePoolable>();
        }

        public void PlaySound(string soundName)
        {
            if (_soundDict.TryGetValue(soundName, out SoundData sound))
            {
                playingSounds.TryAdd(sound.name, AudioManager.Instance.Play(sound.sound, sound.mixer, sound.loop));
            }
            else
            {
                Debug.Log($"Sound {soundName} does not exist in the AudioWrapper.");
            }
        }

        public void PlaySound(string soundName, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(PlayDelayed(soundName, delay));
            }
            else
            {
                PlaySound(soundName);
            }
        }

        private IEnumerator PlayDelayed(string soundName, float delay)
        {
            yield return new WaitForSeconds(delay);

            PlaySound(soundName);
        }

        public void StopSound(string soundName)
        {
            if (playingSounds.TryGetValue(soundName, out AudioSourcePoolable source))
            {
                AudioManager.Instance.TryStopSound(source);
                playingSounds.Remove(soundName);
            }
        }
    }
}