using UnityEditor;
using Native.Component;
using UnityEngine;
using Native.Event;

namespace Native.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    public class ResourceComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private ResourceComponent _resourceComponent;
        private SerializedProperty IsUseEdit;

        private int _languageIndex;
        private string[] _languages;
        private bool _initLanguage;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _resourceComponent = target as ResourceComponent;
            IsUseEdit = serializedObject.FindProperty("IsUseEdit");
            _initLanguage = false;
        }

        private void InitLanguage()
        {
            _languages = new string[EnumExtension.GetCount(typeof(LanguageType))];
            var values = System.Enum.GetNames(typeof(LanguageType));
            for (int i = 0; i < values.Length; i++)
            {
                _languages[i] = values[i];
                if (values[i] == ConfigManager.Instance.CurrentLanguageType.ToString())
                {
                    _languageIndex = i;
                }
            }
            _initLanguage = true;
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying
                && _resourceComponent != null)
            {
                _resourceComponent.Manager.InspectorDraw();
            }

            if (!Application.isPlaying)
            {
                GUILayout.BeginHorizontal("Box");
                IsUseEdit.boolValue = GUILayout.Toggle(IsUseEdit.boolValue, "Use editor load type");
                GUILayout.EndHorizontal();
            }
            else
            {
                if (ConfigManager.Instance.LoadConfigDone && !_initLanguage)
                {
                    InitLanguage();
                }

                if (_initLanguage)
                {
                    GUILayout.BeginHorizontal("Box");
                    GUILayout.Label("Language:");
                    _languageIndex = EditorGUILayout.Popup(_languageIndex, _languages);
                    ConfigManager.Instance.CurrentLanguageType = (LanguageType)System.Enum.Parse(typeof(LanguageType), _languages[_languageIndex]);
                    GUILayout.EndHorizontal();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}