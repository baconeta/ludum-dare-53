using System.Collections;
using ObjectPooling;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourcePoolable : MonoBehaviour, IPoolableExecution, IRecyclable
    {
        private Poolable _poolable;
        private AudioSource _self;

        public void PoolableExecution(Poolable p)
        {
            _poolable = p;
        }

        public void Recycle()
        {
            RemoveFromScene();
        }

        private void RemoveFromScene()
        {
            if (_poolable)
                _poolable.Recycle();
            else
                DefaultDestruction(gameObject);
        }

        private void DefaultDestruction(Object toDestroy)
        {
            Destroy(toDestroy);
        }

        public void Init(AudioMixerGroup group)
        {
            if (!_self) _self = GetComponent<AudioSource>();

            _self.outputAudioMixerGroup = group;
        }

        public void PlayOnce(AudioClip clip)
        {
            if (!_self) _self = GetComponent<AudioSource>();
            _self.PlayOneShot(clip);
            StartCoroutine(ResetObject(clip.length + 0.5f));
        }

        public void PlayLooping(AudioClip clip)
        {
            if (!_self) _self = GetComponent<AudioSource>();
            _self.clip = clip;
            _self.loop = true;
            _self.Play();
        }

        public void StopAudio()
        {
            StartCoroutine(ResetObject(0f));
        }

        private IEnumerator ResetObject(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Now recycle the object
            RemoveFromScene();
        }
    }
}