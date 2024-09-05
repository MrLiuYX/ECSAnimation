using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Collections;
using System.Linq;
using Unity.VisualScripting;
using System;
using PlasticPipe.PlasticProtocol.Messages;

[CustomEditor(typeof(ECSAnimationMaker))]
public class ECSAnimationMakerInspector : Editor
{
    private GameObject _target;
    private SerializedProperty _makeDatas;
    private SkinnedMeshRenderer _sk;
    private MeshFilter _meshFilter;
    private Shader _shader;

    public void OnEnable()
    {
        _target = (serializedObject.targetObject as ECSAnimationMaker).gameObject;
        _makeDatas = serializedObject.FindProperty("MakeDatas");
        _shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/AssetBundleRes/Main/Shaders/AnimationBatchesMerge.shader");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!CheckGOVaild()) return;
        if (GUILayout.Button("Create"))
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_target);
            if (!File.Exists(path))
            {
                Debug.LogError($"Can't find prefab orgin");
                return;
            }

            if (!CheckDataVaild()) return;
            CreateECSObject(path);
        }
    }

    public bool CheckGOVaild()
    {
        _sk = _target.GetComponentInChildren<SkinnedMeshRenderer>();
        _meshFilter = _target.GetComponentInChildren<MeshFilter>();
        return _sk != null || _meshFilter != null;
    }

    public bool CheckDataVaild()
    {
        //if (_makeDatas.arraySize == 0)
        //{
        //    Debug.LogError($"No Make Data");
        //    return false;
        //}

        HashSet<int> ids = new HashSet<int>();

        for (int i = 0; i < _makeDatas.arraySize; i++)
        {
            var id = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Id");

            if (!ids.Add(id.intValue))
            {
                Debug.LogError($"[Element{i}] Id:{id.intValue} has already in make data.");
                return false;
            }

            var animation = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Animation");
            if (animation.objectReferenceValue == null)
            {
                Debug.LogError($"[Element{i}] not set animation clip.");
                return false;
            }
        }

        return true;
    }

    public void CreateECSObject(string path)
    {
        var name = $"ECS_{Path.GetFileNameWithoutExtension(path)}";
        var go = new GameObject(name);
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        var width = _sk.sharedMesh.vertexCount;
        var height = 0;

        for (int i = 0; i < _makeDatas.arraySize; i++)
        {
            var clip = (_makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Animation").objectReferenceValue as AnimationClip);
            _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Start").intValue = height;
            height += Mathf.CeilToInt(clip.length / ECSAnimationMaker.Split);
            _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("End").intValue = height;
        }

        if (height == 0) height = 1;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;
        NativeArray<ECSAnimationMaker.TexData> _datas = new NativeArray<ECSAnimationMaker.TexData>(width * height, Allocator.Temp);

        if (_sk != null)
        {
            int index = 0;
            var mesh = new Mesh();
            for (int i = 0; i < _makeDatas.arraySize; i++)
            {
                var clip = (_makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Animation").objectReferenceValue as AnimationClip);
                _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("LenSec").floatValue = clip.length;
                var jMax = Mathf.CeilToInt(clip.length / ECSAnimationMaker.Split);
                for (float j = 0; j < jMax; j++)
                {
                    clip.SampleAnimation(_target, Mathf.Clamp(j * ECSAnimationMaker.Split, 0, clip.length));
                    _sk.BakeMesh(mesh, false);
                    for (int count = 0; count < mesh.vertexCount; count++)
                    {
                        var v3 = mesh.vertices[count];
                        _datas[index++] = new ECSAnimationMaker.TexData
                        {
                            r = v3.x,
                            g = v3.y,
                            b = v3.z,
                            a = 0,
                        };
                    }
                }
            }

            if (index != width * height)
            {
                _sk.BakeMesh(mesh, false);
                for (int count = 0; count < mesh.vertexCount; count++)
                {
                    var v3 = mesh.vertices[count];
                    _datas[index++] = new ECSAnimationMaker.TexData
                    {
                        r = v3.x,
                        g = v3.y,
                        b = v3.z,
                        a = 0,
                    };
                }
            }
        }
        else
        {
            for (int i = 0; i < _meshFilter.sharedMesh.vertexCount; i++)
            {
                var v3 = _meshFilter.sharedMesh.vertices[i];
                _datas[i++] = new ECSAnimationMaker.TexData
                {
                    r = v3.x,
                    g = v3.y,
                    b = v3.z,
                    a = 0,
                };
            }
        }

        //生成ECS组件
        var authoring = go.AddComponent<ECSAnimationAuthoring>();
        authoring.Config = new EntityAnimationConfigComponentData
        {
            Width = width,
            Height = height,
        };

        List<EntityAnimationConfigBuffer> configBuffer = new List<EntityAnimationConfigBuffer>();
        for (int i = 0; i < _makeDatas.arraySize; i++)
        {
            configBuffer.Add(new EntityAnimationConfigBuffer()
            {
                AnimationId = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Id").intValue,
                AnimationLoop = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Loop").boolValue,
                StartLine = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Start").intValue,
                EndLine = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("End").intValue,
                TotalSec = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("LenSec").floatValue,
            });
        }
        authoring.animationConfigs = configBuffer;

        List<EntityAnimationEventConfigBuffer> eventConfigBuffer = new List<EntityAnimationEventConfigBuffer>();
        for (int i = 0; i < _makeDatas.arraySize; i++)
        {
            var events = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Events");
            for (int j = 0; j < events.arraySize; j++)
            {
                eventConfigBuffer.Add(new EntityAnimationEventConfigBuffer()
                {
                    AnimationId = _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Id").intValue,
                    EventId = events.GetArrayElementAtIndex(j).FindPropertyRelative("EventId").intValue,
                    NormalizeTriggerTime = events.GetArrayElementAtIndex(j).FindPropertyRelative("EventTime").floatValue,
                });
            }
        }
        authoring.eventConfigs = eventConfigBuffer;

        var mat = new Material(_shader);

        texture.SetPixelData(_datas, 0);
        texture.Apply();

        mat.SetTexture("_VertexDataTex", texture);
        mat.SetFloat("_UVX", 1f / width);

        _datas.Dispose();

        var meshPath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_mesh.mesh").Replace("/", "\\");
        var matPath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_mat.mat").Replace("/", "\\");
        var texturePath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_DataTex.asset").Replace("/", "\\");
        var prefabPath = Path.Combine("Assets/ECSBakePrefabs", $"{name}_Prefab.prefab").Replace("/", "\\");        
        if (File.Exists(meshPath)) File.Delete(meshPath);
        if (File.Exists(matPath)) File.Delete(matPath);
        if (File.Exists(texturePath)) File.Delete(texturePath);
        AssetDatabase.Refresh();

        if (_sk)
        {
            var mesh = new Mesh();
            _sk.BakeMesh(mesh);
            AssetDatabase.CreateAsset(mesh, InternalCheckPathVaild(meshPath));            
        }
        else
        {
            AssetDatabase.CreateAsset(_meshFilter.mesh, InternalCheckPathVaild(meshPath));
        }

        authoring.MeshPath = meshPath;
        authoring.MatPath = matPath;

        serializedObject.ApplyModifiedProperties();
        
        AssetDatabase.CreateAsset(mat, InternalCheckPathVaild(matPath));
        
        PrefabUtility.SaveAsPrefabAsset(go, InternalCheckPathVaild(prefabPath));
        
        AssetDatabase.CreateAsset(texture, InternalCheckPathVaild(texturePath));
        
        AssetDatabase.Refresh();
                
        GameObject.DestroyImmediate(go);
    }

    private string InternalCheckPathVaild(string path)
    {
        if(!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        return path;
    }
}
