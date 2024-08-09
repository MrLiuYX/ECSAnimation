using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Native.Resource
{
    public partial class ResourceManager
    {
        public override void Init()
        {
            base.Init();
            _allLoadAssets = new Dictionary<AssetInfo, Object>();
            _allLoadResouces = new Dictionary<ResourceInfo, AssetBundle>();
            _allAssetInfos = new HashSet<AssetInfo>();
            _allResouceInfos = new HashSet<ResourceInfo>();
            _loadAssets = new HashSet<string>();
            _loadResouces = new HashSet<string>();
            _allAssetAgents = new HashSet<LoadAssetsAgent>();
            _allResourceAgents = new HashSet<LoadResourceAgent>();
            _allLoadBinaryAssets = new Dictionary<string, byte[]>();            
        }

        public override void OnUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            for (int i = 0; i < _allAssetAgents.Count; i++)
            {
                _allAssetAgents.ElementAt(i).Excute(elapseSecond);
            }
            for (int i = 0; i < _allResourceAgents.Count; i++)
            {
                _allResourceAgents.ElementAt(i).Excute(elapseSecond);
            }
        }
    }
}
