//using System.IO;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class ShaderVariantCollector : EditorWindow
//{
//    [MenuItem("Tools/Shader Variant Collector")]
//    public static void ShowWindow()
//    {
//        GetWindow<ShaderVariantCollector>("Shader Variant Collector");
//    }

//    private string targetDirectory = "Assets/Shaders";
//    private string shaderCollectionName = "MyShaderCollection";

//    private void OnGUI()
//    {
//        EditorGUILayout.LabelField("Shader Variant Collector", EditorStyles.boldLabel);

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
//        targetDirectory = EditorGUILayout.TextField("Target Directory", targetDirectory);
//        shaderCollectionName = EditorGUILayout.TextField("Shader Collection Name", shaderCollectionName);

//        EditorGUILayout.Space();
//        if (GUILayout.Button("Collect Shaders and Set to ProjectSettings"))
//        {
//            CollectShadersAndSetToProjectSettings();
//        }
//    }

//    private void CollectShadersAndSetToProjectSettings()
//    {
//        ShaderVariantCollection shaderCollection = new ShaderVariantCollection();
//        CollectShadersRecursive(targetDirectory, shaderCollection);
//        string shaderCollectionPath = Path.Combine(targetDirectory, shaderCollectionName + ".shadervariants");
//        AssetDatabase.CreateAsset(shaderCollection, shaderCollectionPath);

//        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
//        GraphicsSettings.graphicsSettings = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderCollectionPath);
//        EditorUtility.SetDirty(GraphicsSettings.graphicsSettings);
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }

//    private void CollectShadersRecursive(string directory, ShaderVariantCollection shaderCollection)
//    {
//        string[] subDirectories = Directory.GetDirectories(directory);
//        foreach (string subDirectory in subDirectories)
//        {
//            CollectShadersRecursive(subDirectory, shaderCollection);
//        }

//        string[] shaderPaths = Directory.GetFiles(directory, "*.shader");
//        foreach (string shaderPath in shaderPaths)
//        {
//            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
//            if (shader != null)
//            {
//                shaderCollection.Add(new ShaderVariantCollection.ShaderVariant(shader, PassType.ForwardBase));
//            }
//        }
//    }
//}