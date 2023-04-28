using UnityEngine;

namespace Utils
{
    public class EverlastingSingleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Singleton pattern
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject newInstance = new();
                        _instance = newInstance.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}