using System.IO;
using UnityEditor;
using UnityEngine;

namespace Native.Editor
{
    public class EventEditor : EditorWindow
    {
        [MenuItem("LCFramework/Tools/CreateEvent")]
        public static void EventCreator()
        {
            var window = EditorWindow.CreateWindow<EventEditor>("Event Creator");
            window.minSize = new UnityEngine.Vector2(400, 600);
        }

        private const string _normalPath = "Assets/Scripts/LCFramework/Runtime/Event/CustomEvents/{0}.cs";
        private string _className = string.Empty;


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("ClassName", GUILayout.Width(100));
                    _className = EditorGUILayout.TextField(_className);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(string.Format(_normalPath, _className));
                if (GUILayout.Button("Create Custom Event Button"))
                {
                    if (EditorUtility.DisplayDialog(
                        "Create custom event"
                        , $"{string.Format(_normalPath, _className)}"
                        , "Create"
                        , "Cancel"))
                    {
                        GeneratorCode();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void GeneratorCode()
        {
            var path = string.Format(_normalPath, _className);
            if (File.Exists(path))
            {
                EditorUtility.DisplayDialog("Create Error", "Can't create this file, Because already has same name file exist", "ok");
                return;
            }
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("using Native.Component;");
            sw.WriteLine();
            sw.WriteLine("namespace Native.Event");
            sw.WriteLine("{");
            sw.WriteLine($"\tpublic class {_className} : EventArgsBase");
            sw.WriteLine("\t{");
            sw.WriteLine($"\t\tprivate static string _eventName = \"{_className}\";");
            sw.WriteLine("\t\tprivate static long _eventId = 0;");
            sw.WriteLine("\t\tpublic static long EventId");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tget");
            sw.WriteLine("\t\t\t{");
            sw.WriteLine("\t\t\t\t_eventId = _eventId == 0 ? Native.Component.LaunchComponent.Event.GetEventId(_eventName) : _eventId;");
            sw.WriteLine("\t\t\t\treturn _eventId;");
            sw.WriteLine("\t\t\t}");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t\tpublic override void Clear()");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tbase.Clear();");
            sw.WriteLine("\t\t}");
            sw.WriteLine($"\t\tpublic static {_className} Create()");
            sw.WriteLine("\t\t{");
            sw.WriteLine($"\t\t\tvar data = ReferencePool.Acquire<{_className}>();");
            sw.WriteLine("\t\t\treturn data;");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
            sw.Dispose();
            AssetDatabase.Refresh();
            return;
        }
    }
}
