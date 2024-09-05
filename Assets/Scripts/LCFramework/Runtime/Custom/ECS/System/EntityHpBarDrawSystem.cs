
using Native.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[UpdateAfter(typeof(EntityHpBarCollectionSystem))]
public partial class EntityHpBarDrawSystem : SystemBase
{
    private Texture2D _texture;
    private int _currentSize;
    private int _currentSizePow2;
    private NativeArray<HpBarComponentData> _texDatas;

    private Material _material;
    private Mesh _mesh;
    private Bounds _bounds;
    private ComputeBuffer _computeBuffer;
    private int _subMeshIndex;
    private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
    private const int _size = 3;
    private bool _loadMat = false;

    protected override void OnCreate()
    {
        base.OnCreate();
        _texDatas = new NativeArray<HpBarComponentData>(_currentSize, Allocator.Persistent);
        _computeBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _texDatas.Dispose();
    }


    protected override void OnUpdate()
    {
        if (_material == null)
        {
            if(!_loadMat)
            {
                _loadMat = true;
                _mesh = new Mesh()
                {
                    vertices = new Vector3[]
                     {
                     new Vector3(-0.7f, -0.2f, 0),
                     new Vector3(0.7f,  -0.2f, 0),
                     new Vector3(-0.7f,  0.2f, 0),
                     new Vector3(0.7f, 0.2f, 0),
                     },

                    uv = new Vector2[]
                     {
                     new Vector2(0, 0),
                     new Vector2(1, 0),
                     new Vector2(0, 1),
                     new Vector2(1, 1),
                     },

                    triangles = new int[]
                     {
                     0,2,1,1,2,3
                     },

                };
                _mesh.RecalculateNormals();
                LaunchComponent.Resource.LoadAsset("Assets/AssetBundleRes/Main/Materials/HpBar.mat", new Native.Resource.LoadAssetCallback(
                (name, asset, time, userData) =>
                {
                    _material = (Material)asset;
                }));
                _bounds = new Bounds(Vector3.zero, Vector3.one * 512);
                _subMeshIndex = Mathf.Clamp(_subMeshIndex, 0, _mesh.subMeshCount - 1);
                _args[0] = (uint)_mesh.GetIndexCount(_subMeshIndex);
                _args[2] = (uint)_mesh.GetIndexStart(_subMeshIndex);
                _args[3] = (uint)_mesh.GetBaseVertex(_subMeshIndex);
            }

            return;
        }

        var _trulyDatas = SystemAPI.GetSingletonRW<HpBarCollectionComponentData>().ValueRW.Datas;
        var trulyDataLength = _trulyDatas.Length;

        if (trulyDataLength == 0) return;

        AutoCreateTex(trulyDataLength);

        for (int i = 0; i < trulyDataLength; i++)
        {
            _texDatas[i] = _trulyDatas[i];

        }

        _texture.SetPixelData(_texDatas, 0);
        _texture.Apply();

        _args[1] = (uint)trulyDataLength;
        _computeBuffer.SetData(_args);
        Graphics.DrawMeshInstancedIndirect(
            _mesh
            , _subMeshIndex
            , _material
            , _bounds
            , _computeBuffer
            , 0
            , null
            , UnityEngine.Rendering.ShadowCastingMode.Off
            , false
            , 0
            , null);
    }

    private void AutoCreateTex(int length)
    {
        var compareValue = length * _size;
        if (_currentSizePow2 >= compareValue) return;

        while (_currentSizePow2 < compareValue)
        {
            if (_currentSize == 0) _currentSize = 1 << 4;
            _currentSize *= 2;
            _currentSizePow2 = Mathf.CeilToInt(math.pow(_currentSize, 2));
        }

        if (_texture != null)
        {
            GameObject.Destroy(_texture);
        }

        _texDatas.Dispose();
        _texDatas = new NativeArray<HpBarComponentData>(_currentSizePow2, Allocator.Persistent);

        _texture = new Texture2D(_currentSize, _currentSize, TextureFormat.RGBAFloat, false);
        _texture.filterMode = FilterMode.Point;

        _material.SetTexture("_DataTex", _texture);
        _material.SetInt("_Size", _currentSize);
    }
}
