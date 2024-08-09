using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Native.Editor
{
    public class UIEditor : UnityEditor.EditorWindow
    {
        [MenuItem("LCFramework/Tools/GeneratorUICode")]
        public static void GeneratorCode()
        {
            var window = EditorWindow.GetWindow<UIEditor>();
            window.minSize = new Vector2(300, 300);
        }

        public static string _UIFormCreateDirectory = "Assets/Scripts/LCFramework/Runtime/UI/CustomUICode/{0}";
        public static string _UIItemCreateDirectory = "Assets/Scripts/LCFramework/Runtime/UI/CustomUIItemCode/{0}";


        public static string UICodeCreateDirectory;
        public static string BindFile = ".BindComponent.cs";
        public static string LogicFile = ".cs";
        private GameObject _prefab;
        private bool _createLogicCode;
        private bool _createBindCode;
        private bool _createUIItem;

        private void OnEnable()
        {
            _prefab = null;
            _createUIItem = false;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            _createUIItem = GUILayout.Toggle(_createUIItem, "´´½¨UIItemn´úÂë");
            UICodeCreateDirectory = _createUIItem ? _UIItemCreateDirectory : _UIFormCreateDirectory;
            _prefab = (GameObject)EditorGUILayout.ObjectField(_prefab, typeof(GameObject), true);
            if (_prefab == null)
            {
                GUILayout.EndVertical();
                return;
            }
            GUILayout.Label($"Create target directory {string.Format(UICodeCreateDirectory, _prefab.name)}");
            _createLogicCode = EditorGUILayout.Toggle("Create logic code", _createLogicCode);
            _createBindCode = EditorGUILayout.Toggle("Create bind code", _createBindCode);
            if (GUILayout.Button("Create"))
            {
                if (_createBindCode)
                {
                    CreateBindCode(_prefab);
                }

                if (_createLogicCode)
                {
                    if (EditorUtility.DisplayDialog("Rewrite Logic!", "Will Clear your current logic, please check this operation right!", "Ensure", "Cancel"))
                    {
                        InternalGeneratorLogicCode();
                    }
                }
                AssetDatabase.Refresh();
            }
            GUILayout.EndVertical();
        }

        private void InternalGeneratorLogicCode()
        {
            var path = Path.Combine(string.Format(UICodeCreateDirectory, _prefab.name), _prefab.name + LogicFile);

            if (!Directory.Exists(string.Format(UICodeCreateDirectory, _prefab.name)))
            {
                Directory.CreateDirectory(string.Format(UICodeCreateDirectory, _prefab.name));
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("using UnityEngine.UI;");
                    sw.WriteLine("using Native.Component;");
                    sw.WriteLine("using Native.Event;");
                    sw.WriteLine();
                    sw.WriteLine("namespace Native.UI");
                    sw.WriteLine("{");
                    if (!_createUIItem)
                        sw.WriteLine($"\tpublic partial class {_prefab.name} : UIFormLogic");
                    else
                        sw.WriteLine($"\tpublic partial class {_prefab.name} : UIItemLogic");
                    sw.WriteLine("\t{");
                    sw.WriteLine();
                    if (_createUIItem)
                    {
                        sw.WriteLine("\t\tpublic override void OnInitBind()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnInitBind();");
                        sw.WriteLine("\t\t\tBindComponents(gameObject);");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic override void OnSpawn(UIFormLogic logic, object userData)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnSpawn(logic, userData);");
                        sw.WriteLine("\t\t\tRegisterEvent();");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic override void OnUnsapwn()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnUnsapwn();");
                        sw.WriteLine("\t\t\tRemoveEvent();");
                        sw.WriteLine("\t\t}");
                    }
                    else
                    {
                        sw.WriteLine("\t\tpublic override void OnInit(object userData)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnInit(userData);");
                        sw.WriteLine("\t\t\tBindComponents(gameObject);");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic override void OnOpen(object userData)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnOpen(userData);");
                        sw.WriteLine("\t\t\tRegisterEvent();");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic override void OnClose(object userData)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.OnClose(userData);");
                        sw.WriteLine("\t\t\tRemoveEvent();");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic override void Destroy()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.Destroy();");
                        sw.WriteLine("\t\t}");
                    }
                    sw.WriteLine();
                    sw.WriteLine("\t\tprivate void RegisterEvent()");
                    sw.WriteLine("\t\t{");
                    GameObject go = _prefab;
                    var autoBindComponent = go.GetComponent<Native.UI.UIAutoBind>();
                    var funcNames = new List<string>();
                    if (autoBindComponent != null)
                    {
                        var bindData = autoBindComponent._bindDatas;
                        for (int i = 0; i < bindData.Count; i++)
                        {
                            if (bindData[i].component.GetType() == typeof(Button))
                            {
                                var funcName = $"OnBtn{bindData[i].componentName.Substring(1, bindData[i].componentName.Length - 1)}Clicked";
                                funcNames.Add(funcName);
                                sw.WriteLine($"\t\t\t{bindData[i].componentName}.onClick.AddListener({funcName});");
                            }
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"{_prefab.name} not has UIAutoBind Component!");
                    }
                    sw.WriteLine("\t\t}");

                    sw.WriteLine("\t\tprivate void RemoveEvent()");
                    sw.WriteLine("\t\t{");
                    if (autoBindComponent != null)
                    {
                        var bindData = autoBindComponent._bindDatas;
                        for (int i = 0; i < bindData.Count; i++)
                        {
                            if (bindData[i].component.GetType() == typeof(Button))
                            {
                                var funcName = $"OnBtn{bindData[i].componentName.Substring(1, bindData[i].componentName.Length - 1)}Clicked";
                                sw.WriteLine($"\t\t\t{bindData[i].componentName}.onClick.RemoveListener({funcName});");
                            }
                        }
                    }
                    sw.WriteLine("\t\t}");
                    for (int i = 0; i < funcNames.Count; i++)
                    {
                        sw.WriteLine();
                        sw.WriteLine($"\t\tprivate void {funcNames[i]}()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\t");
                        sw.WriteLine("\t\t}");
                    }
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                }
            }
        }

        private void RegisterEvent()
        {

        }

        private void RemoveEvent()
        {

        }

        public static void CreateBindCode(GameObject prefab)
        {
            bool uiitem = !prefab.name.Contains("UIForm");
            UICodeCreateDirectory = uiitem ? _UIItemCreateDirectory : _UIFormCreateDirectory;

            var path = Path.Combine(string.Format(UICodeCreateDirectory, prefab.name), prefab.name + BindFile);

            if (!Directory.Exists(string.Format(UICodeCreateDirectory, prefab.name)))
            {
                Directory.CreateDirectory(string.Format(UICodeCreateDirectory, prefab.name));
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    GameObject go = prefab;
                    var autoBindComponent = go.GetComponent<Native.UI.UIAutoBind>();
                    var bindData = autoBindComponent._bindDatas;
                    sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("using UnityEngine.UI;");
                    sw.WriteLine();
                    sw.WriteLine("namespace Native.UI");
                    sw.WriteLine("{");
                    if (uiitem)
                        sw.WriteLine($"\tpublic partial class {prefab.name}");
                    else
                        sw.WriteLine($"\tpublic partial class {prefab.name}");
                    sw.WriteLine("\t{");
                    if (bindData != null)
                        for (int i = 0; i < bindData.Count; i++)
                        {
                            sw.WriteLine($"\t\tprivate {bindData[i].component.GetType().Name} {bindData[i].componentName};");
                        }
                    sw.WriteLine();
                    sw.WriteLine("\t\tpublic virtual void BindComponents(GameObject go)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tvar autoBindComponent = go.GetComponent<UIAutoBind>();");
                    if(bindData != null)
                    for (int i = 0; i < bindData.Count; i++)
                    {
                        sw.WriteLine($"\t\t\t{bindData[i].componentName} = autoBindComponent.GetAutoBindComponent<{bindData[i].component.GetType().Name}>({i});");
                    }
                    sw.WriteLine("\t\t}");

                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                }
            }
        }
    }
}
