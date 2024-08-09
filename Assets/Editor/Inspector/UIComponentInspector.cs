using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Native.Component;
using Native.UI;
using Native.Editor;

[CustomEditor(typeof(UIComponent))]
public class UIComponentInspector : UnityEditor.Editor
{
    private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
    private UIComponent _uiComponent;
    private SerializedProperty _groupNames;
    private SerializedProperty _groupDeepLayers;
    private SerializedProperty _uiFormDestroyTime;

    public void OnEnable()
    {
        _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
        _uiComponent = target as UIComponent;
        _groupNames = serializedObject.FindProperty("_groupNames");
        _groupDeepLayers = serializedObject.FindProperty("_groupDeepLayers");
        _uiFormDestroyTime = serializedObject.FindProperty("_uiFormDestroyTime");
    }

    public override void OnInspectorGUI()
    {
        _componentCommonPropertyInspector.Draw();
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Groups");
                if (!Application.isPlaying && GUILayout.Button("+"))
                {
                    AddGroup();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            {
                GUILayout.Label("界面销毁时间", GUILayout.Width(120));
                if (!Application.isPlaying)
                {
                    var tempStr = GUILayout.TextField(_uiFormDestroyTime.floatValue.ToString());
                    if (int.TryParse(tempStr, out var result))
                    {
                        _uiFormDestroyTime.floatValue = result;
                    }
                    else
                    {
                        _uiFormDestroyTime.floatValue = 60;
                    }
                }
                else
                {
                    GUILayout.Label(_uiFormDestroyTime.floatValue.ToString());
                }
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < _groupNames.arraySize; i++)
            {
                GUILayout.BeginVertical("Box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label($"GroupName", GUILayout.Width(120));
                        if (!Application.isPlaying)
                            _groupNames.GetArrayElementAtIndex(i).stringValue = GUILayout.TextField(_groupNames.GetArrayElementAtIndex(i).stringValue);
                        else
                            GUILayout.Label(_groupNames.GetArrayElementAtIndex(i).stringValue);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label($"GroupDeepLayer", GUILayout.Width(120));
                        if (!Application.isPlaying)
                        {
                            var temp = GUILayout.TextField(_groupDeepLayers.GetArrayElementAtIndex(i).intValue.ToString());
                            if (int.TryParse(temp, out int value))
                            {
                                value = value > (1 << 16 - 1) ? (1 << 16 - 1) : value;
                                _groupDeepLayers.GetArrayElementAtIndex(i).intValue = value;
                            }
                            else
                            {
                                _groupDeepLayers.GetArrayElementAtIndex(i).intValue = 0;
                            }
                        }
                        else
                        {
                            GUILayout.Label(_groupDeepLayers.GetArrayElementAtIndex(i).intValue.ToString());
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button("-"))
                        {
                            MinusGroup(i);
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

    private void AddGroup()
    {
        var index = _groupNames.arraySize;
        _groupNames.InsertArrayElementAtIndex(index);
        _groupDeepLayers.InsertArrayElementAtIndex(index);
        _groupNames.GetArrayElementAtIndex(index).stringValue = string.Empty;
        _groupDeepLayers.GetArrayElementAtIndex(index).intValue = default(int);
    }

    private void MinusGroup(int index)
    {
        if (!EditorUtility.DisplayDialog($"提示", $"删除UI组{_groupNames.GetArrayElementAtIndex(index).stringValue}", "ok", "cancel"))
            return;
        _groupNames.DeleteArrayElementAtIndex(index);
        _groupDeepLayers.DeleteArrayElementAtIndex(index);
    }
}
