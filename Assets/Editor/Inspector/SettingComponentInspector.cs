using UnityEditor;
using Native.Component;
using UnityEngine;
using System.IO;

namespace Native.Editor
{
    [CustomEditor(typeof(SettingComponent))]
    public class SettingComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private SettingComponent _settingComponent;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _settingComponent = target as SettingComponent;
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying)
                _settingComponent.Manager.InspectorDraw();
            if(GUILayout.Button("Clear"))
            {
                File.Delete(Path.Combine(Application.persistentDataPath, "SaveGameData.setting"));
                Debug.Log("Clear done!");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

