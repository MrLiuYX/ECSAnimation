using UnityEngine;
using UnityEditor;

namespace Native.Editor
{
    [CustomEditor(typeof(ReferencePoolComponent))]
    public class ReferencePoolInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
                ReferencePool.InspectorDraw();
        }
    }
}
