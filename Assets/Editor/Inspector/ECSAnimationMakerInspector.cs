using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Linq;

[CustomEditor(typeof(ECSAnimationMaker))]
public class ECSAnimationMakerInspector : Editor
{
    private GameObject _target;
    private SerializedProperty _makeDatas;
    private Renderer[] _allSkin;
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
        var temp = _target.GetComponentsInChildren<Renderer>().ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] is ParticleSystemRenderer)
            {
                temp.RemoveAt(i--);
            }
        }
        _allSkin = temp.ToArray();
        return _allSkin != null;
    }

    public bool CheckDataVaild()
    {
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
        var name = $"ECS_{Path.GetFileNameWithoutExtension(path).Replace(" ", "_")}";
        var go = new GameObject(name);
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        try
        {
            //生成ECS组件
            var authoring = go.AddComponent<ECSAnimationAuthoring>();
            authoring.TexConfigs = new EntityAnimationConfigComponentData[_allSkin.Length];
            authoring.MeshPath = new string[_allSkin.Length];
            authoring.MatPath = new string[_allSkin.Length];
            for (int i = 0; i < _allSkin.Length; i++)
            {
                int width = 0;
                int height = 0;
                string meshPath = "";
                string matPath = "";
                Bake(name, i, _allSkin[i], ref width, ref height, ref meshPath, ref matPath);

                authoring.TexConfigs[i] = new EntityAnimationConfigComponentData
                {
                    Width = width,
                    Height = height,
                };

                authoring.MeshPath[i] = meshPath;

                authoring.MatPath[i] = matPath;
            }

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

            authoring.animationConfigs = configBuffer;

            authoring.eventConfigs = eventConfigBuffer;

            var prefabPath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSBakePrefabs", $"{name}_Prefab.prefab");

            PrefabUtility.SaveAsPrefabAsset(go, InternalCheckPathVaild(prefabPath));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Create fail : {e.Message}");
            throw;
        }
        finally
        {
            GameObject.DestroyImmediate(go);
        }

        Debug.Log("Create Success");
    }

    private void Bake(string name, int path, Renderer _sk, ref int width, ref int height, ref string meshPath, ref string matPath)
    {
        Mesh staticMesh;
        GameObject meshAdpater = null;
        Transform targetTs = null;
        var useMeshRenderer = false;
        Texture mainTex = null;
        var mainColor = Color.white;
        if (_sk as SkinnedMeshRenderer)
        {
            staticMesh = GameObject.Instantiate((_sk as SkinnedMeshRenderer).sharedMesh);
            targetTs = _sk.transform;
            mainTex = (_sk as SkinnedMeshRenderer).sharedMaterials[0].GetTexture("_MainTex") 
                ?? (_sk as SkinnedMeshRenderer).sharedMaterials[0].GetTexture("_BaseMap");
            mainColor = (_sk as SkinnedMeshRenderer).sharedMaterials[0].color;
        }
        else
        {
            meshAdpater = new GameObject("MeshAdpater");
            targetTs = _sk.transform;
            var shareMesh = GameObject.Instantiate(_sk.GetComponent<MeshFilter>().sharedMesh);
            mainTex = (_sk as MeshRenderer).sharedMaterials[0].GetTexture("_MainTex")
                ?? (_sk as MeshRenderer).sharedMaterials[0].GetTexture("_BaseMap");
            mainColor = (_sk as MeshRenderer).sharedMaterials[0].color;
            _sk = meshAdpater.AddComponent<SkinnedMeshRenderer>();
            (_sk as SkinnedMeshRenderer).sharedMesh = shareMesh;
            staticMesh = shareMesh;
            useMeshRenderer = true;
        }

        width = _sk != null ? staticMesh.vertexCount : staticMesh.vertexCount;

        height = 0;

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

        Texture2D normalTexture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;

        NativeArray<ECSAnimationMaker.TexData> verticesData = new NativeArray<ECSAnimationMaker.TexData>(width * height, Allocator.Temp);
        NativeArray<ECSAnimationMaker.TexData> normalsData = new NativeArray<ECSAnimationMaker.TexData>(width * height, Allocator.Temp);

        if (_sk as SkinnedMeshRenderer)
        {
            int index = 0;
            int index_normal = 0;
            for (int i = 0; i < _makeDatas.arraySize; i++)
            {
                var clip = (_makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Animation").objectReferenceValue as AnimationClip);
                _makeDatas.GetArrayElementAtIndex(i).FindPropertyRelative("LenSec").floatValue = clip.length;
                var jMax = Mathf.CeilToInt(clip.length / ECSAnimationMaker.Split);
                for (float j = 0; j < jMax; j++)
                {
                    var samplePoint = Mathf.Clamp(j * ECSAnimationMaker.Split, 0, clip.length);
                    clip.SampleAnimation(_target, samplePoint);
                    Mesh mesh;
                    if (useMeshRenderer)
                    {
                        mesh = GameObject.Instantiate(targetTs.GetComponent<MeshFilter>().sharedMesh);
                    }
                    else
                    {
                        mesh = new Mesh();
                        (_sk as SkinnedMeshRenderer).BakeMesh(mesh, false);
                    }


                    if (_sk.transform.parent != null && _sk.transform.parent != _target.transform && _sk.transform.parent.GetComponent<ECSAnimationMaker>() != null)
                    {
                        //var a = 0;
                        //对子物体重新采样 生成对应的Mesh
                        var childECSMaker = _sk.transform.parent.GetComponent<ECSAnimationMaker>();
                        if (childECSMaker.MakeDatas.Count != 0)
                        {
                            var childMakeData = childECSMaker.MakeDatas[0];

                            var second = childMakeData.Animation.length;
                            var childSamplePoint = (second * samplePoint) / clip.length;
                            childMakeData.Animation.SampleAnimation(_sk.transform.parent.gameObject, childSamplePoint);
                            (_sk as SkinnedMeshRenderer).BakeMesh(mesh, false);
                        }
                    }

                    for (int count = 0; count < mesh.vertexCount; count++)
                    {

                        Matrix4x4 localToWorld = targetTs.localToWorldMatrix;
                        var v3 = localToWorld.MultiplyPoint3x4(mesh.vertices[count]);
                        verticesData[index++] = new ECSAnimationMaker.TexData
                        {
                            r = v3.x, //+ offset.x,
                            g = v3.y,// + offset.y,
                            b = v3.z,// + offset.z,
                            a = 0,
                        };
                    }

                    mesh.RecalculateNormals();

                    for (int count = 0; count < mesh.normals.Length; count++)
                    {
                        Matrix4x4 localToWorld = targetTs.localToWorldMatrix;
                        var v3 = localToWorld.MultiplyPoint3x4(mesh.normals[count]);
                        normalsData[index_normal++] = new ECSAnimationMaker.TexData
                        {
                            r = v3.x,// + offset.x,
                            g = v3.y,// + offset.y,
                            b = v3.z,// + offset.z,
                            a = 0,
                        };
                    }
                }
            }

            if (index == 0)
            {
                var mesh = new Mesh();
                (_sk as SkinnedMeshRenderer).BakeMesh(mesh, false);
                mesh.RecalculateNormals();
                for (int count = 0; count < mesh.vertexCount; count++)
                {
                    Matrix4x4 localToWorld = targetTs.localToWorldMatrix;
                    var v3 = localToWorld.MultiplyPoint3x4(mesh.vertices[count]);
                    verticesData[index++] = new ECSAnimationMaker.TexData
                    {
                        r = v3.x,// + offset.x,
                        g = v3.y,// + offset.y,
                        b = v3.z,// + offset.z,
                        a = 0,
                    };
                }

                for (int count = 0; count < mesh.normals.Length; count++)
                {
                    Matrix4x4 localToWorld = targetTs.localToWorldMatrix;
                    var v3 = localToWorld.MultiplyPoint3x4(mesh.normals[count]);
                    normalsData[index_normal++] = new ECSAnimationMaker.TexData
                    {
                        r = v3.x,// + offset.x,
                        g = v3.y,// + offset.y,
                        b = v3.z,// + offset.z,
                        a = 0,
                    };
                }
            }
        }

        var mat = new Material(_shader);
        texture.SetPixelData(verticesData, 0);
        texture.Apply();

        normalTexture.SetPixelData(normalsData, 0);
        normalTexture.Apply();

        verticesData.Dispose();

        meshPath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_{path}_mesh.mesh");
        matPath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_{path}_mat.mat");
        var texturePath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_{path}_DataTex.asset");
        var normalTexturePath = Path.Combine("Assets/AssetBundleRes/Main/Prefabs/ECSRendererData", name, $"{name}_{path}_NormalTex.asset");
        if (File.Exists(meshPath)) File.Delete(meshPath);
        if (File.Exists(matPath)) File.Delete(matPath);
        if (File.Exists(texturePath)) File.Delete(texturePath);
        if (File.Exists(normalTexturePath)) File.Delete(normalTexturePath);
        AssetDatabase.Refresh();

        if (_sk as SkinnedMeshRenderer)
        {
            AssetDatabase.CreateAsset(staticMesh, InternalCheckPathVaild(meshPath));
        }


        serializedObject.ApplyModifiedProperties();

        AssetDatabase.CreateAsset(texture, InternalCheckPathVaild(texturePath));

        AssetDatabase.CreateAsset(normalTexture, InternalCheckPathVaild(normalTexturePath));

        AssetDatabase.Refresh();

        mat.SetTexture("_VertexDataTex", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
        mat.SetTexture("_NormalTex", AssetDatabase.LoadAssetAtPath<Texture2D>(normalTexturePath));
        mat.SetFloat("_UVX", 1f / width);
        mat.SetColor("_MainColor", mainColor);
        if (mainTex != null) mat.SetTexture("_MainTex", mainTex);

        AssetDatabase.CreateAsset(mat, InternalCheckPathVaild(matPath));

        AssetDatabase.Refresh();

        GameObject.DestroyImmediate(meshAdpater);
    }

    private float3 CalcOffset(float3 posOffset, float3 rotOffset, float3 scaleOffset, float3 orgin)
    {
        float3x3 M_Scale = new float3x3
                            (
                                scaleOffset.x, 0, 0,
                                0, scaleOffset.y, 0,
                                0, 0, scaleOffset.z
                            );

        orgin = math.mul(M_Scale, orgin);

        float3x3 M_rotateX = new float3x3
        (
            1, 0, 0,
            0, math.cos(rotOffset.x), -math.sin(rotOffset.x),
            0, math.sin(rotOffset.x), math.cos(rotOffset.x)
        );
        float3x3 M_rotateY = new float3x3
        (
            math.cos(rotOffset.y), 0, math.sin(rotOffset.y),
            0, 1, 0,
            -math.sin(rotOffset.y), 0, math.cos(rotOffset.y)
        );

        float3x3 M_rotateZ = new float3x3
        (
            math.cos(rotOffset.z), -math.sin(rotOffset.z), 0,
            math.sin(rotOffset.z), math.cos(rotOffset.z), 0,
            0, 0, 1
        );

        orgin = math.mul(M_rotateZ, orgin);
        orgin = math.mul(M_rotateY, orgin);
        orgin = math.mul(M_rotateX, orgin);
        orgin += posOffset;
        return orgin;
    }

    private string InternalCheckPathVaild(string path)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        return path;
    }
}
