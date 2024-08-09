using LitJson;
using Native.Resource;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Native.Editor
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("LCFramework/Tools/BuildAB")]
        public static void BuildABWindow()
        {
            var window = EditorWindow.CreateWindow<AssetBundleEditor>("AssetBundle Tool");
            window.minSize = new Vector2(200, 200);
        }

        private const string _buildFilter = "t:Scene t:Prefab t:Shader t:Model t:Material t:Texture t:AudioClip t:AnimationClip t:AnimatorController t:Font t:TextAsset t:ScriptableObject t:ShaderVariantCollection t:videoclip t:Mesh";
        private static string[] _buildPlatformStr = new string[]
        {
            "Windows32",
            "Windows64",
            "Mac",
            "IOS",
            "Android",
            "WebGL",
        };
        private static string[] _binaryAsset = new string[]
        {
            "dat",
            "data",
        };
        private Dictionary<string, BuildTarget> _buildPlatformDict = new Dictionary<string, BuildTarget>()
        {
            {"Windows32", BuildTarget.StandaloneWindows },
            {"Windows64", BuildTarget.StandaloneWindows64 },
            {"Mac", BuildTarget.StandaloneOSX },
            {"IOS", BuildTarget.iOS },
            {"Android", BuildTarget.Android },
            { "WebGL", BuildTarget.WebGL},
        };
        private int _buildPlatformIndex;

        private static List<AssetInfo> _assetInfos = new List<AssetInfo>();
        private static List<ResourceInfo> _resourceInfos = new List<ResourceInfo>();

        private void OnEnable()
        {
#if UNITY_ANDROID
            _buildPlatformIndex = 4;
#else
            _buildPlatformIndex = 1;
#endif
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Build Platform", GUILayout.Width(100));
                    _buildPlatformIndex = EditorGUILayout.Popup(_buildPlatformIndex, _buildPlatformStr);
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Build"))
                {
                    if (EditorUtility.DisplayDialog("Build Tool", "Build AssetBundle", "Run", "Cancel"))
                    {
                        Build(_buildPlatformDict[_buildPlatformStr[_buildPlatformIndex]]);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        public static void Build(BuildTarget buildPlatform)
        {
            if (Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.Delete(Application.streamingAssetsPath, true);
            }
            else
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            AssetDatabase.Refresh();

            var folders = new string[] { "Assets/AssetBundleRes" };
            var guids = AssetDatabase.FindAssets(_buildFilter, folders);
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                assetPath = assetPath.Replace('\\', '/');
                AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
                assetImporter.assetBundleName = assetPath.Replace("/" + Path.GetFileName(assetPath), string.Empty);
                assetImporter.assetBundleVariant = "unity3d_lc";
                if(assetPath.ToLower().Contains("GPUPrefabs".ToLower()))
                {
                    try
                    {
                        assetImporter.assetBundleName = string.Empty;
                        assetImporter.assetBundleVariant = string.Empty;
                    }
                    catch (System.Exception)
                    {
                        Debug.LogWarning($"{assetPath} to None");
                    }
                }
            }

            BuildPipeline.BuildAssetBundles(
                Application.streamingAssetsPath
                , BuildAssetBundleOptions.ChunkBasedCompression
                , buildPlatform);

            AssetDatabase.Refresh();

            CreateAssetInfoAndResourceInfo();

            Directory.Delete(Application.persistentDataPath, true);

            AssetDatabase.Refresh();

            Debug.Log("Build Success");
        }

        private static void CreateAssetInfoAndResourceInfo()
        {
            _resourceInfos.Clear();
            _assetInfos.Clear();
            var temp = AssetBundle.LoadFromFile("Assets/StreamingAssets/StreamingAssets");
            var manifest = temp.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var all = manifest.GetAllAssetBundles();
            for (int i = 0; i < all.Length; i++)
            {
                var tempResourceInfo = new ResourceInfo();
                all[i] = all[i].Replace('\\', '/');
                var depend = manifest.GetAllDependencies(all[i]);
                tempResourceInfo.ResoucePath = all[i];
                tempResourceInfo.DependResourcePath = new string[depend.Length];
                for (int j = 0; j < depend.Length; j++)
                {
                    tempResourceInfo.DependResourcePath[j] = depend[j];
                }
                _resourceInfos.Add(tempResourceInfo);
            }

            var folders = new string[] { "Assets/AssetBundleRes" };
            var guids = AssetDatabase.FindAssets(_buildFilter, folders);
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                assetPath = assetPath.Replace('\\', '/');
                var tempAssetInfo = new AssetInfo();
                tempAssetInfo.AssetMode = (int)AssetInfo.AssetModeEnum.Normal;
                tempAssetInfo.AssetPath = assetPath;
                tempAssetInfo.ResoucePath = assetPath.Replace("/" + Path.GetFileName(assetPath), string.Empty) + ".unity3d_lc";
                tempAssetInfo.ResoucePath = tempAssetInfo.ResoucePath.ToLower();
                _assetInfos.Add(tempAssetInfo);
            }

            //二进制文件
            var paths = AssetDatabase.GetAllAssetPaths().ToList();
            for (int i = 0; i < paths.Count; i++)
            {
                if (_assetInfos.Find(x => x.AssetPath == paths[i]) != null)
                {
                    paths.RemoveAt(i--);
                    continue;
                }

                if (!paths[i].Contains("Assets/AssetBundleRes/Main/Binary"))
                {
                    paths.RemoveAt(i--);
                    continue;
                }

                if (FileUtils.IsDirectory(paths[i]))
                {
                    paths.RemoveAt(i--);
                    continue;
                }
            }

            for (int i = 0; i < paths.Count; i++)
            {
                var tempAssetInfo = new AssetInfo();
                tempAssetInfo.AssetMode = (int)AssetInfo.AssetModeEnum.Binary;
                tempAssetInfo.AssetPath = paths[i];
                _assetInfos.Add(tempAssetInfo);

                FileUtils.CopyFile(paths[i], Path.Combine(Application.streamingAssetsPath, paths[i]));
            }
            AssetDatabase.Refresh();

            var asset = JsonMapper.ToJson(_assetInfos);
            var resource = JsonMapper.ToJson(_resourceInfos);
            var assetPath2 = Path.Combine(Application.streamingAssetsPath, "AssetInfo.txt");
            var resourcePath = Path.Combine(Application.streamingAssetsPath, "ResourceInfo.txt");
            File.Create(assetPath2).Dispose();
            File.Create(resourcePath).Dispose();

            FileStream fs = new FileStream(assetPath2, FileMode.Open);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(asset);
            sw.Flush();
            sw.Dispose();
            fs.Dispose();

            fs = new FileStream(resourcePath, FileMode.Open);
            sw = new StreamWriter(fs);
            sw.Write(resource);
            sw.Flush();
            sw.Dispose();
            fs.Dispose();
        }
    }
}
