using UnityEditor;
using Native.Component;
using UnityEngine;
using System.IO;

namespace Native.Editor
{
    [CustomEditor(typeof(AudioComponent))]
    public class AudioComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private AudioComponent _audioComponent;
        private SerializedProperty _maxAudioChanel;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _audioComponent = target as AudioComponent;
            _maxAudioChanel = serializedObject.FindProperty("_maxAudioChanel");
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying)
                _audioComponent.Manager.InspectorDraw();

            if (!Application.isPlaying)
                _maxAudioChanel.intValue = EditorGUILayout.IntField("每个音频轨道最大播放数量:", _maxAudioChanel.intValue);
            else
                GUILayout.Label($"每个音频轨道最大播放数量:{_maxAudioChanel}");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
