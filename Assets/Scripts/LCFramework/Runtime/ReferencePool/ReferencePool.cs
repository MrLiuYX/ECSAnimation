using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace Native
{
    public static class ReferencePool
    {
        private static List<ReferenceCollection> _referenceCollections;

        static ReferencePool()
        {
            _referenceCollections = new List<ReferenceCollection>();
        }

        public static T Acquire<T>() where T : class, IReference, new()
        {
            var temp = _referenceCollections.Find(x => x.CheckType(typeof(T)));
            if (temp == null)
            {
                temp = new ReferenceCollection(typeof(T));
                _referenceCollections.Add(temp);
            }
            return (T)temp.Acquire();
        }

        public static void Release<T>(T t, bool createIfNeed = true) where T : IReference
        {
            var temp = _referenceCollections.Find(x => x.CheckType(t.GetType()));
            if (temp == null
                && !createIfNeed)
            {
                UnityEngine.Debug.LogError($"lyx Can't find type {t.GetType().FullName} in referencepoolCollection");
                return;
            }
            if (temp == null)
            {
                temp = new ReferenceCollection(t.GetType());
                _referenceCollections.Add(temp);
            }
            temp.Release(t);
        }

        public static StringBuilder Info()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _referenceCollections.Count; i++)
            {
                _referenceCollections[i].Info(sb);
            }
            return sb;
        }

        public static void InspectorDraw()
        {
            for (int i = 0; i < _referenceCollections.Count; i++)
            {
                _referenceCollections[i].InspectorDraw();
            }
        }
    }
}
