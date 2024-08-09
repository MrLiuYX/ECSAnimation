using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native
{
    public interface IPool<T>
    {
        public int UseCount { get; }
        public int FreeCount { get; }

        public T Spawn();

        public void UnSpawnAll();

        public void UnSpawn(T instance);
    }
}
