using Audio;
using ObjectPooling;
using UI.Settings;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    public sealed class AudioManager : EverlastingSingleton<AudioManager>
    {
        [SerializeField] [Min(3)] private int simultaneousSoundLimit = 8;
        [SerializeField] private GameObject audioSourceObject;

        [Header("Mixers")] [SerializeField] private AudioMixer masterMixer;

        private ObjectPool _audioSources;

        public const string MusicKey = "MusicVolume";
        public const string SfxKey = "SfxVolume";
        public const string AmbientKey = "AmbientVolume";

        /// <summary>
        /// Note that the object returned from this call will nullify itself if not looping
        /// </summary>
        public AudioSourcePoolable Play(AudioClip clip, AudioMixerGroup mixerGroup, bool looping = true)
        {
            AudioSourcePoolable audioSource = GetAudioSourceFromPool();
            audioSource.Init(mixerGroup);
            if (looping)
            {
                audioSource.PlayLooping(clip);
            }
            else
            {
                audioSource.PlayOnce(clip);
            }

            return audioSource;
        }

        protected override void Awake()
        {
            base.Awake();
            // Setup object pooling for audio sources
            _audioSources = ObjectPool.Build(audioSourceObject, simultaneousSoundLimit, simultaneousSoundLimit);
            LoadVolumes();
        }

        public void TryStopSound(AudioSourcePoolable source)
        {
            if (source != null && source.isActiveAndEnabled)
            {
                source.StopAudio();
            }
        }

        private AudioSourcePoolable GetAudioSourceFromPool()
        {
            return _audioSources.GetRecyclable<AudioSourcePoolable>();
        }

        private void LoadVolumes() // Volume is saved in VolumeSettings.cs
        {
            float musicVol = PlayerPrefs.GetFloat(MusicKey, 0.5f);
            float sfxVol = PlayerPrefs.GetFloat(SfxKey, 0.5f);
            float ambientVol = PlayerPrefs.GetFloat(AmbientKey, 0.5f);

            masterMixer.SetFloat(VolumeSettings.MixerMusic, Mathf.Log10(musicVol) * 20);
            masterMixer.SetFloat(VolumeSettings.SfxMusic, Mathf.Log10(sfxVol) * 20);
            masterMixer.SetFloat(VolumeSettings.AmbientMusic, Mathf.Log10(ambientVol) * 20);
        }
    }
}