using System.IO;
using UnityEditor;
using UnityEngine;

namespace Native.Editor
{
    public class ProcedureEditor : EditorWindow
    {
        [MenuItem("LCFramework/Tools/CreateProcedure")]
        public static void ProcedureCreator()
        {
            var window = EditorWindow.CreateWindow<ProcedureEditor>("Procedure Creator");
            window.minSize = new UnityEngine.Vector2(400, 600);
        }

        private const string _normalPath = "Assets/Scripts/LCFramework/Runtime/Procedure/CustomProcedure/{0}.cs";
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
                if (GUILayout.Button("Create Custom Procedure Button"))
                {
                    if (EditorUtility.DisplayDialog(
                        "Create custom Procedure"
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
            sw.WriteLine("using Native.FSM;");
            sw.WriteLine("using Native.Component;");
            sw.WriteLine("using Native.Event;");
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine();
            sw.WriteLine("namespace Native.Procedure");
            sw.WriteLine("{");
            sw.WriteLine($"\tpublic class {_className} : ProcedureBase");
            sw.WriteLine("\t{");

            sw.WriteLine("\t\tpublic override void OnInit(IFSM fsm)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tbase.OnInit(fsm);");
            sw.WriteLine("\t\t}");
            sw.WriteLine();

            sw.WriteLine("\t\tpublic override void OnEnter(IFSM fsm)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tbase.OnEnter(fsm);");
            sw.WriteLine("\t\t}");
            sw.WriteLine();

            sw.WriteLine("\t\tpublic override void OnUpdate(IFSM fsm, float elpaseSecond)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tbase.OnUpdate(fsm, elpaseSecond);");
            sw.WriteLine("\t\t}");
            sw.WriteLine();

            sw.WriteLine("\t\tpublic override void OnExit(IFSM fsm)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tbase.OnExit(fsm);");
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
