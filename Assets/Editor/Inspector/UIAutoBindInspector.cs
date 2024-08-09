using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using Native.UI;
using System;
using C = UnityEngine.Component;

namespace Native.Editor
{
    [CustomEditor(typeof(UIAutoBind))]
    public class UIAutoBindInspector : UnityEditor.Editor
    {
        private SerializedProperty _bindDatas;
        private bool _drawRuler;
        private Dictionary<string, Type> _bindRuler = new Dictionary<string, Type>()
        {
            { "_txt", typeof(Text)},
            { "_img", typeof(Image)},
            { "_rect", typeof(RectTransform)},
            { "_btn", typeof(Button)},
            { "_horLayout",typeof(HorizontalLayoutGroup)},
            { "_verLayout",typeof(VerticalLayoutGroup)},
            { "_rawimg",typeof(RawImage)},
            { "_input",typeof(InputField)},
            { "_dropdown",typeof(Dropdown)},
        };
        private List<string> _tempBindName;

        private void OnEnable()
        {
            _bindDatas = serializedObject.FindProperty("_bindDatas");
            _tempBindName = new List<string>();
            _drawRuler = false;

            for (int i = 0; i < _bindDatas.arraySize; i++)
            {
                _tempBindName.Add(_bindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("componentName").stringValue);
            }
        }

        public override void OnInspectorGUI()
        {
            if (!((UIAutoBind)serializedObject.targetObject).gameObject.name.Contains("UIForm")
                && !((UIAutoBind)serializedObject.targetObject).gameObject.name.Contains("UIItem"))
            {
                GUILayout.Label($"命名必须以UIForm或者UIItem结尾");
                return;
            }

            DrawRuler();
            DrawBind();
        }

        private void DrawRuler()
        {
            GUILayout.BeginVertical("Box");

            if (GUILayout.Button(_drawRuler ? "关闭绑定规则" : "打开绑定规则"))
            {
                _drawRuler = !_drawRuler;
            }

            if (_drawRuler)
            {
                foreach (var kv in _bindRuler)
                {
                    GUILayout.Label($"{kv.Key}\t{kv.Value.ToString()}");
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawBind()
        {
            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("绑定组件"))
            {
                var uiitem = !((UIAutoBind)serializedObject.targetObject).gameObject.name.Contains("UIForm");
                BindComponent();
                var objName = (target as UIAutoBind).gameObject.name;
                var path = (string.Format(uiitem ? UIEditor._UIItemCreateDirectory : UIEditor._UIFormCreateDirectory, objName) + "/" + objName + UIEditor.BindFile);
                if (File.Exists(path))
                {
                    UIEditor.CreateBindCode((target as UIAutoBind).gameObject);
                }
            }
            for (int i = 0; i < _bindDatas.arraySize; i++)
            {
                var property = _bindDatas.GetArrayElementAtIndex(i);
                var name = property.FindPropertyRelative("componentName").stringValue;
                GUILayout.BeginHorizontal();
                GUILayout.Label(name, GUILayout.Width(150));
                EditorGUILayout.ObjectField(property.FindPropertyRelative("component"));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void BindComponent()
        {
            _bindDatas.ClearArray();
            _tempBindName.Clear();
            var gameObject = (serializedObject.targetObject as UIAutoBind).gameObject;
            RecursionBind(gameObject.transform);
            serializedObject.ApplyModifiedProperties();
        }

        private void RecursionBind(Transform root)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                RecursionBind(root.GetChild(i));
            }

            foreach (var kv in _bindRuler)
            {
                if (root.name.Contains(kv.Key))
                {
                    var component = root.GetComponent(kv.Value);
                    if (component != null && _tempBindName.Contains(root.name))
                    {
                        UnityEngine.Debug.LogError($"[UIAutoBindInspector.RecursionBind] Already bind {root.name}. Check has same name.");
                        goto JumpToForeach;
                    }

                    if (component == null)
                    {
                        UnityEngine.Debug.LogError($"[UIAutoBindInspector.RecursionBind] Can't bind {root.name} obj. Check has component {kv.Value.ToString()}.");
                        goto JumpToForeach;
                    }

                    var index = _bindDatas.arraySize;
                    _bindDatas.InsertArrayElementAtIndex(index);
                    var property = _bindDatas.GetArrayElementAtIndex(index);
                    property.FindPropertyRelative("componentName").stringValue = root.name;
                    property.FindPropertyRelative("component").objectReferenceValue = component;
                    _tempBindName.Add(root.name);

                JumpToForeach:
                    break;
                }
            }
        }
    }
}
