
using Native.Component;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimationRendererAssetDictComponentData : IComponentData
{
    public enum AssetState
    {
        None,
        Loading,
        Loadone,
    }

    public class DictData
    {
        public string MeshPath;
        public string MatPath;
        public Mesh Mesh;
        public Material Mat;
        public AssetState State;

        public void StartLoad()
        {
            State = AssetState.Loading;

            LaunchComponent.Resource.LoadAsset(MeshPath.Replace("\\", "/"), new Native.Resource.LoadAssetCallback(
                (_, asset, _, _) =>
                {
                    Mesh = asset as Mesh;
                    if (CheckLoadDone()) State = AssetState.Loadone;
                }));

            LaunchComponent.Resource.LoadAsset(MatPath.Replace("\\", "/"), new Native.Resource.LoadAssetCallback(
                (_, asset, _, _) =>
                {
                    Mat = asset as Material;
                    if (CheckLoadDone()) State = AssetState.Loadone;
                }));
        }

        private bool CheckLoadDone()
        {
            return Mesh != null && Mat != null;
        }
    }

    /// <summary>
    /// MeshPath, MatPath
    /// </summary>
    public Dictionary<(string, string), int> AssetPathDict;
    public Dictionary<int, DictData> AssetDict;
    public int DictCount;
}