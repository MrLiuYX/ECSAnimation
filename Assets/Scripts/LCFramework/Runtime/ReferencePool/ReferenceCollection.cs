using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace Native
{
    public class ReferenceCollection
    {
        private Type _type;
        private Stack<IReference> _free;
        private int _usingCount;

        public ReferenceCollection(Type type)
        {
            _type = type;
            _free = new Stack<IReference>();
        }

        public IReference Acquire()
        {
            IReference temp = null;
            if (_free.Count == 0)
            {
                temp = (IReference)System.Activator.CreateInstance(_type);
            }
            else
            {
                temp = _free.Pop();
            }
            _usingCount++;
            return temp;
        }

        public void Release(IReference reference)
        {
            if (_free.Contains(reference))
            {
                Debug.LogError($"Can't release reference {reference.GetType().Name}");
                return;
            }

            reference.Clear();
            _free.Push(reference);
            _usingCount--;
        }

        /// <summary>
        /// 检测type类型是否一致
        /// </summary>
        public bool CheckType(Type type)
        {
            return _type == type;
        }

        public StringBuilder Info(StringBuilder sb)
        {
            sb.Append($"{Environment.NewLine}Type:{_type.FullName}{Environment.NewLine}use count:{_usingCount}{Environment.NewLine}free count:{_free.Count}");
            return sb;
        }

        public void InspectorDraw()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label($"Type:{_type.FullName}");
            GUILayout.Label($"Use number:{_usingCount}");
            GUILayout.Label($"Free number:{_free.Count}");
            GUILayout.Label($"Total number:{_free.Count + _usingCount}");
            GUILayout.EndVertical();
        }
    }
}
