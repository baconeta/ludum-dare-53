using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ObjectPooling
{
    [Serializable]
    [ExecuteInEditMode]
    public sealed class Poolable : Parkable, IRecyclable
    {
        [SerializeField] [HideInInspector] private ObjectPool pool;

        private static bool _scriptBuiltInstance;

        private void Awake()
        {
            InstantiationGuard();
            DontDestroyOnLoad(this); // Hack: Keep objects around in cache between scenes... this isn't very efficient
            ExecuteEvents.Execute<IPoolableExecution>(gameObject, null,
                (script, ignored) => script.PoolableExecution(this));
        }

        private void InstantiationGuard()
        {
            if (!_scriptBuiltInstance)
            {
                DestroyImmediate(this, true);
                throw new InvalidOperationException("Can only be created with AddPoolableComponent");
            }

            _scriptBuiltInstance = false;
        }

        private void OnEnable()
        {
            gameObject.hideFlags = 0;
        }

        private void OnDisable()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        public void Recycle()
        {
            pool.Recycle(this);
        }

        public static Poolable AddPoolableComponent(GameObject newInstance, ObjectPool pool)
        {
            _scriptBuiltInstance = true;
            Poolable instance = newInstance.AddComponent<Poolable>();
            instance.pool = pool;
            return instance;
        }
    }

    public interface IPoolableExecution : IEventSystemHandler
    {
        void PoolableExecution(Poolable p);
    }

    public interface IRecyclable
    {
        void Recycle();
    }

    public abstract class Parkable : MonoBehaviour
    {
        public virtual void Park()
        {
            gameObject.SetActive(false);
        }

        public virtual void Unpark()
        {
            gameObject.SetActive(true);
        }
    }
}