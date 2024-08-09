using UnityEditor;
using Native.Component;
using UnityEngine;

namespace Native.Editor
{
    [CustomEditor(typeof(FSMComponent))]
    public class FSMComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private FSMComponent _targetComponent;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _targetComponent = target as FSMComponent;
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying
                && _targetComponent != null)
            {
                _targetComponent.Manager.InspectorDraw();
            }
        }
    }
}

