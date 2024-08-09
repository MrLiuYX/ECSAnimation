using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Native.Component;
using UnityEngine;
using Native.Procedure;

namespace Native.Editor
{
    [CustomEditor(typeof(ProcedureComponent))]
    public class ProcedureComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private ProcedureComponent _targetComponent;
        private SerializedProperty ProcedureTypes;
        private SerializedProperty StartProcedure;

        private List<string> _allProcedure;
        private List<string> _chooseProcedureList;
        private Dictionary<string, bool> _chooseProcedureState;
        private int _startIndex;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _targetComponent = target as ProcedureComponent;
            _allProcedure = new List<string>();
            _chooseProcedureState = new Dictionary<string, bool>();
            _chooseProcedureList = new List<string>();
            ProcedureTypes = serializedObject.FindProperty("_procedureTypes");
            StartProcedure = serializedObject.FindProperty("_startProcedure");
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                //if (item.GetName().Name == "Native")
                //{
                    foreach (var type in item.GetTypes())
                    {
                        if (typeof(ProcedureBase).IsAssignableFrom(type)
                            && type != typeof(ProcedureBase))
                        {
                            _allProcedure.Add(type.FullName);
                            _chooseProcedureState.Add(type.FullName, false);
                        }
                    }
                //}
            }

            for (int i = 0; i < ProcedureTypes.arraySize; i++)
            {
                var element = ProcedureTypes.GetArrayElementAtIndex(i);
                _chooseProcedureState[element.stringValue] = true;
            }

            for (int i = 0; i < ProcedureTypes.arraySize; i++)
            {
                var type = ProcedureTypes.GetArrayElementAtIndex(i);
                if (!_allProcedure.Contains(type.stringValue))
                {
                    ProcedureTypes.DeleteArrayElementAtIndex(i--);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshSelectIndex(List<string> procedures)
        {
            for (int i = 0; i < procedures.Count; i++)
            {
                if (procedures[i] == StartProcedure.stringValue)
                {
                    _startIndex = i;
                    return;
                }
            }
            _startIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying
                && _targetComponent != null)
            {
                _targetComponent.Manager.InspectorDraw();
            }

            if (!Application.isPlaying)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("所有流程:");
                    for (int i = 0; i < _allProcedure.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label($"{i + 1}. {_allProcedure[i]}");
                            var temp = _chooseProcedureState[_allProcedure[i]] = GUILayout.Toggle(_chooseProcedureState[_allProcedure[i]], "Use");
                            if (temp)
                            {
                                if (ProcedureTypes.arraySize == 0)
                                {
                                    ProcedureTypes.InsertArrayElementAtIndex(0);
                                    var element = ProcedureTypes.GetArrayElementAtIndex(0);
                                    element.stringValue = _allProcedure[i];
                                }
                                else
                                {
                                    var needInsert = true;
                                    for (int j = 0; j < ProcedureTypes.arraySize; j++)
                                    {
                                        if (ProcedureTypes.GetArrayElementAtIndex(j).stringValue == _allProcedure[i])
                                        {
                                            needInsert = false;
                                            break;
                                        }
                                    }
                                    if (needInsert)
                                    {
                                        ProcedureTypes.InsertArrayElementAtIndex(ProcedureTypes.arraySize - 1);
                                        var element = ProcedureTypes.GetArrayElementAtIndex(ProcedureTypes.arraySize - 1);
                                        element.stringValue = _allProcedure[i];
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < ProcedureTypes.arraySize; j++)
                                {
                                    if (ProcedureTypes.GetArrayElementAtIndex(j).stringValue == _allProcedure[i])
                                    {
                                        ProcedureTypes.DeleteArrayElementAtIndex(j);
                                        break;
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    {
                        _chooseProcedureList.Clear();
                        _chooseProcedureList.AddRange(_allProcedure);
                        for (int i = 0; i < _chooseProcedureList.Count; i++)
                        {
                            if (!_chooseProcedureState[_chooseProcedureList[i]])
                                _chooseProcedureList.RemoveAt(i--);
                        }
                        if (_chooseProcedureList.Count != 0)
                        {
                            RefreshSelectIndex(_chooseProcedureList);
                            GUILayout.Label("Start Procedure", GUILayout.Width(100));
                            _startIndex = EditorGUILayout.Popup(_startIndex, _chooseProcedureList.ToArray());
                            StartProcedure.stringValue = _chooseProcedureList[_startIndex];
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

