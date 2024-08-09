using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static AnimationRendererAssetDictComponentData;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[UpdateAfter(typeof(EntityAnimationSystem))]
public partial class EntityAnimationDrawSystem : SystemBase
{
    private class AnimationDrawComponentData
    {
        public DictData AssetData;
        public NativeList<AnimationInstanceComponentData> _trulyDatas;

        private Texture2D _texture;
        private int _currentSize;
        private int _currentSizePow2;
        private NativeArray<AnimationInstanceComponentData> _texDatas;

        private Material _material;
        private Mesh _mesh;
        private Bounds _bounds;
        private ComputeBuffer _computeBuffer;
        private int _subMeshIndex;
        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
        private const int _size = 2;

        public void OnCreate()
        {
            _texDatas = new NativeArray<AnimationInstanceComponentData>(_currentSize, Allocator.Persistent);
            _trulyDatas = new NativeList<AnimationInstanceComponentData>(Allocator.Persistent);
            _computeBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        }

        public void OnDestory()
        {
            _texDatas.Dispose();
        }

        public void OnUpdate()
        {
            if (_material == null)
            {
                if(AssetData.State == AssetState.None)
                {
                    AssetData.StartLoad();
                    return;
                }

                if(AssetData.State == AssetState.Loading)
                {
                    return;
                }

                _mesh = AssetData.Mesh;
                _material = AssetData.Mat;
#if UNITY_EDITOR   
                //Editor上用Shader读一遍 不然Editor上AB加载不到
                var shader = Shader.Find("Unlit/Animation");
                _material.shader = shader;
#endif

                _bounds = new Bounds(Vector3.zero, Vector3.one * 512);
                _subMeshIndex = Mathf.Clamp(_subMeshIndex, 0, _mesh.subMeshCount - 1);
                _args[0] = (uint)_mesh.GetIndexCount(_subMeshIndex);
                _args[2] = (uint)_mesh.GetIndexStart(_subMeshIndex);
                _args[3] = (uint)_mesh.GetBaseVertex(_subMeshIndex);
            }

            var trulyDataLength = _trulyDatas.Length;

            AutoCreateTex(trulyDataLength);

            for (int i = 0; i < trulyDataLength; i++)
            {
                _texDatas[i] = _trulyDatas[i];
            }

            _texture.SetPixelData(_texDatas, 0);
            _texture.Apply();

            _args[1] = (uint)trulyDataLength;
            _computeBuffer.SetData(_args);
            //Debug.LogError($"Draw {trulyDataLength}");
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
            _texDatas = new NativeArray<AnimationInstanceComponentData>(_currentSizePow2, Allocator.Persistent);

            _texture = new Texture2D(_currentSize, _currentSize, TextureFormat.RGBAFloat, false);
            _texture.filterMode = FilterMode.Point;

            _material.SetTexture("_InstanceDataTex", _texture);
            _material.SetInt("_Size", _currentSize);
        }
    }

    private Dictionary<int, AnimationDrawComponentData> _allDraw;
    private AnimationRendererAssetDictComponentData _assetData;

    protected override void OnCreate()
    {
        _allDraw = new Dictionary<int, AnimationDrawComponentData>();        
    }

    protected override void OnDestroy()
    {
        foreach (var item in _allDraw.Values)
        {
            item.OnDestory();
        }
        _allDraw.Clear();
    }


    protected override void OnUpdate()
    {
        if(_assetData == null) _assetData = EntityManager.GetComponentObject<AnimationRendererAssetDictComponentData>(SystemAPI.GetSingleton<EntityPoolTag>().Owner);
        var animInstanceDatas = SystemAPI.GetSingleton<AnimationInstanceDatas>();

        foreach (var item in _allDraw.Values)
        {
            item._trulyDatas.Clear();
        }

        for (int i = 0; i < animInstanceDatas.Datas.Length; i++)
        {
            var data = animInstanceDatas.Datas[i];
            var MeshAndMatIndex = data.MeshAndMatIndex;
            
            if (!_allDraw.ContainsKey(MeshAndMatIndex))
            {
                var createNew = new AnimationDrawComponentData();
                createNew.AssetData = _assetData.AssetDict[MeshAndMatIndex];
                createNew.OnCreate();
                _allDraw.Add(MeshAndMatIndex, createNew);
            }

            var draw = _allDraw[MeshAndMatIndex];
            draw._trulyDatas.Add(data.InstanceData);
        }

        foreach (var item in _allDraw.Values)
        {
            item.OnUpdate();
        }
    }
}