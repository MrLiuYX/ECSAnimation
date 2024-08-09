using UnityEditor;
using Native.Component;
using UnityEngine;

namespace Native.Editor
{
    [CustomEditor(typeof(EventComponent))]
    public class EventComponentInspector : UnityEditor.Editor
    {
        private ComponentCommonPropertyInspector _componentCommonPropertyInspector;
        private EventComponent _eventComponent;

        public void OnEnable()
        {
            _componentCommonPropertyInspector = new ComponentCommonPropertyInspector(serializedObject);
            _eventComponent = target as EventComponent;
        }

        public override void OnInspectorGUI()
        {
            _componentCommonPropertyInspector.Draw();
            if (Application.isPlaying
                && _eventComponent != null)
            {
                _eventComponent.Manager.InspectorDraw();
            }
        }
    }
}

