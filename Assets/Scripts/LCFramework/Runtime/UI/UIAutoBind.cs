using UnityEngine;
using System;
using System.Collections.Generic;

namespace Native.UI
{
    public class UIAutoBind : MonoBehaviour
    {
        [Serializable]
        public sealed class BindData
        {
            public UnityEngine.Component component;
            public string componentName;
        }

        [SerializeField]
        public List<BindData> _bindDatas;

        public T GetAutoBindComponent<T>(int index) where T : UnityEngine.Component
        {
            if (index < 0 || index >= _bindDatas.Count)
                throw new IndexOutOfRangeException();

            return (T)_bindDatas[index].component;
        }
    }
}
