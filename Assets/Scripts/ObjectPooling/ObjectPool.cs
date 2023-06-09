﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class ObjectPool : ScriptableObject
    {
        [SerializeField] [HideInInspector] GameObject prefab;

        [SerializeField] [HideInInspector] ParkingStorage parking;

        public bool IsEmpty => parking.IsEmpty;

        public void Recycle(Poolable p)
        {
            parking.Park(p);
        }

        public T GetRecyclable<T>() where T : IRecyclable
        {
            return GetRecyclable().GetComponent<T>();
        }

        public Poolable GetRecyclable()
        {
            if (parking.IsEmpty)
                return Clone();
            else
                return (Poolable) parking.Unpark();
        }

        public static ObjectPool Build(GameObject prefab, int initialClones, int initialCapacity)
        {
            ObjectPool pool = CreateInstance<ObjectPool>();
            pool.Initialize(prefab, initialClones, initialCapacity);
            return pool;
        }

        private void Initialize(GameObject prefabObject, int initialClones, int capacity)
        {
            prefab = prefabObject;
            parking = ParkingStorage.InfiniteSpace(capacity);
            ParkInitialClones(initialClones);
        }

        private void ParkInitialClones(int initialClones)
        {
            for (int i = 0; i < initialClones; ++i)
                parking.Park(Clone());
        }

        private Poolable Clone()
        {
            GameObject clone = Instantiate(prefab);
            Poolable p = Poolable.AddPoolableComponent(clone, this);
            return p;
        }
    }

    [Serializable]
    public class ParkingStorage
    {
        [SerializeField] [HideInInspector] private List<Parkable> fauxStack;

        ParkingStorage()
        {
        }

        public static ParkingStorage InfiniteSpace(int capacity)
        {
            ParkingStorage p = new()
            {
                fauxStack = new List<Parkable>(capacity)
            };
            return p;
        }

        public bool IsEmpty => fauxStack.Count == 0;

        public void Park(Parkable p)
        {
            PushFauxStack(p);
            p.Park();
        }

        public Parkable Unpark()
        {
            Parkable p = Pop();
            p.Unpark();
            return p;
        }

        private void PushFauxStack(Parkable p)
        {
            fauxStack.Add(p);
        }

        private Parkable Pop()
        {
            var lastIndex = fauxStack.Count - 1;
            Parkable p = fauxStack[lastIndex];
            fauxStack.RemoveAt(lastIndex);
            return p;
        }
    }
}