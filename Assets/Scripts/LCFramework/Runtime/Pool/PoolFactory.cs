using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Native
{
    public static class PoolFactory<T> where T : class, new()
    {
        public static IPool<T> Create(Func<T> createNew)
        {
            var instance = Activator.CreateInstance(typeof(PoolBase<T>), createNew, null, null);
            return (IPool<T>)instance;
        }

        public static IPool<T> Create(Func<T> createNew, Action<T> spawnAction)
        {
            var instance = Activator.CreateInstance(typeof(PoolBase<T>), createNew, spawnAction, null);
            return (IPool<T>)instance;
        }

        public static IPool<T> Create(Func<T> createNew, Action<T> spawnAction, Action<T> unspawnAction)
        {
            var instance = Activator.CreateInstance(typeof(PoolBase<T>), createNew, spawnAction, unspawnAction);
            return (IPool<T>)instance;
        }
    }

    public class PoolBase<T> : IPool<T> where T : class, new()
    {
        private Action<T> _spawnAction;
        private Action<T> _unspawnAction;
        private Func<T> _createNew;
        private Queue<T> _free;
        private List<T> _use;
        private GameObject _poolParent;

        public int UseCount { get; private set; }

        public int FreeCount { get; private set; }

        public PoolBase(Func<T> createNew, Action<T> spawnAction, Action<T> unspawnAction)
        {
            _free = new Queue<T>();
            _use = new List<T>();
            _createNew = createNew;
            _spawnAction = spawnAction;
            _unspawnAction = unspawnAction;
        }

        public T Spawn()
        {
            T back = null;
            if (_free.Count != 0)
            {
                back = _free.Dequeue();
                FreeCount--;
            }
            else
            {
                back = _createNew?.Invoke();
            }
            _spawnAction?.Invoke(back);
            UseCount++;
            _use.Add(back);
            return back;
        }

        public void UnSpawn(T instance)
        {
            _unspawnAction?.Invoke(instance);
            _free.Enqueue(instance);
            _use.Remove(instance);
            UseCount--;
            FreeCount++;
        }

        public void UnSpawnAll()
        {
            for (int i = 0; i < _use.Count; i++)
            {
                UnSpawn(_use[i--]);                
            }
        }
    }
}
