using UnityEngine;
using UnityEditor;

namespace Native.Editor
{
    public class ComponentCommonPropertyInspector
    {
        private SerializedProperty _orderSerializeProperty;
        private SerializedProperty _enableUpdateSerializeProperty;
        private SerializedProperty _enableFixedUpdateSerializeProperty;
        private SerializedProperty _enableLateUpdateSerializeProperty;
        private SerializedObject _serializedObject;

        private int _cacheOrder;
        private bool _cacheEnableUpdate;
        private bool _cacheEnableFixedUpdate;
        private bool _cacheEnableLateUpdate;

        public ComponentCommonPropertyInspector(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _orderSerializeProperty = serializedObject.FindProperty("_order");
            _enableUpdateSerializeProperty = serializedObject.FindProperty("_enableUpdate");
            _enableFixedUpdateSerializeProperty = serializedObject.FindProperty("_enableFixedUpdate");
            _enableLateUpdateSerializeProperty = serializedObject.FindProperty("_enableLateUpdate");

            _cacheOrder = _orderSerializeProperty.intValue;
            _cacheOrder = _cacheOrder == 0 ? 1 : _cacheOrder;
            _cacheEnableUpdate = _enableUpdateSerializeProperty.boolValue;
            _cacheEnableFixedUpdate = _enableFixedUpdateSerializeProperty.boolValue;
            _cacheEnableLateUpdate = _enableLateUpdateSerializeProperty.boolValue;
        }

        public void Draw()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("管理器公用属性");
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("管理器顺序", GUILayout.Width(65));
                    GUILayout.Label($"{_cacheOrder}", GUILayout.Width(40));
                    if (GUILayout.Button("↑"))
                    {
                        _cacheOrder = _cacheOrder << 1 >= 65536 ? 65536 : _cacheOrder << 1;
                    }
                    if (GUILayout.Button("↓"))
                    {
                        _cacheOrder = _cacheOrder >> 1 <= 0 ? 1 : _cacheOrder >> 1;
                    }
                    _orderSerializeProperty.intValue = _cacheOrder;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("管理器Update轮训", GUILayout.Width(150));
                    _cacheEnableUpdate = EditorGUILayout.Toggle(_cacheEnableUpdate);
                    _enableUpdateSerializeProperty.boolValue = _cacheEnableUpdate;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("管理器FixedUpdate轮训", GUILayout.Width(150));
                    _cacheEnableFixedUpdate = EditorGUILayout.Toggle(_cacheEnableFixedUpdate);
                    _enableFixedUpdateSerializeProperty.boolValue = _cacheEnableFixedUpdate;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("管理器LateUpdate轮训", GUILayout.Width(150));
                    _cacheEnableLateUpdate = EditorGUILayout.Toggle(_cacheEnableLateUpdate);
                    _enableLateUpdateSerializeProperty.boolValue = _cacheEnableLateUpdate;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
