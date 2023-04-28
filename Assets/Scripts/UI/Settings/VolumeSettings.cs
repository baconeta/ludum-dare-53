using System;
using Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Settings
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider ambientSlider;

        public const string MixerMusic = "MusicVolume";
        public const string SfxMusic = "SFXVolume";
        public const string AmbientMusic = "AmbientVolume";

        private void Awake()
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
        }

        private void Start()
        {
            musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MusicKey, 0.5f);
            sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SfxKey, 0.5f);
            ambientSlider.value = PlayerPrefs.GetFloat(AudioManager.AmbientKey, 0.5f);
        }

        private void SetMusicVolume(float value)
        {
            mixer.SetFloat(MixerMusic, Mathf.Log10(value) * 20);
        }

        private void SetSfxVolume(float value)
        {
            mixer.SetFloat(SfxMusic, Mathf.Log10(value) * 20);
        }

        private void SetAmbientVolume(float value)
        {
            mixer.SetFloat(AmbientMusic, Mathf.Log10(value) * 20);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetFloat(AudioManager.MusicKey, musicSlider.value);
            PlayerPrefs.SetFloat(AudioManager.SfxKey, sfxSlider.value);
            PlayerPrefs.SetFloat(AudioManager.AmbientKey, ambientSlider.value);
        }
    }
}