using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [Serializable]
    public struct SoundData
    {
        public string name;
        public AudioClip sound;
        public AudioMixerGroup mixer;
        public bool loop;
        [Range(0.01f, 1.0f)] public float volume;
    }
}