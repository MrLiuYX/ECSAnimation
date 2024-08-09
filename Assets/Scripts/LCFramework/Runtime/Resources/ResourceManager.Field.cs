using System.Collections.Generic;
using UnityEngine;

namespace Native.Resource
{
    public partial class ResourceManager
    {
        public ResourceHelper ResourceHelper;

        private List<string> _needCopyAssetDict;
        private int _needCopyAssetCount;
        private int _copyAssetDoneCount;

        private HashSet<AssetInfo> _allAssetInfos;
        private static HashSet<ResourceInfo> _allResouceInfos;

        private Dictionary<AssetInfo, Object> _allLoadAssets;
        private Dictionary<ResourceInfo, AssetBundle> _allLoadResouces;
        private Dictionary<string, byte[]> _allLoadBinaryAssets;
        private HashSet<string> _loadAssets;
        private HashSet<string> _loadResouces;

        private HashSet<LoadAssetsAgent> _allAssetAgents;
        private HashSet<LoadResourceAgent> _allResourceAgents;

        private const string AssetInfoPath = "AssetInfo.txt";
        private const string ResourceInfoPath = "ResourceInfo.txt";

        public Dictionary<AssetInfo, Object> AllLoadAssets { get { return _allLoadAssets; } }
        public Dictionary<ResourceInfo, AssetBundle> AllLoadResouces { get { return _allLoadResouces; } }
        public HashSet<ResourceManager.LoadAssetsAgent> AllAssetAgent { get { return _allAssetAgents; } }
        public HashSet<ResourceManager.LoadResourceAgent> AllResouceAgent { get { return _allResourceAgents; } }

        public bool IsUseEditLoadType;

        public string PersistenPath { get; set; }
    }
}