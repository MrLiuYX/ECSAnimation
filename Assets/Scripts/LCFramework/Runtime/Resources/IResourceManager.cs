using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Native.Resource
{
    public interface IResourceManager
    {
        public Dictionary<AssetInfo, Object> AllLoadAssets { get; }
        public Dictionary<ResourceInfo, AssetBundle> AllLoadResouces { get; }
        public HashSet<ResourceManager.LoadAssetsAgent> AllAssetAgent { get; }
        public HashSet<ResourceManager.LoadResourceAgent> AllResouceAgent { get; }

        public string PersistenPath { get; set; }

        /// <summary>
        /// 检测资源是否存在
        /// </summary>
        bool CheckHasAsset(string assetPath);

        /// <summary>
        /// 检测AB是否存在
        /// </summary>
        /// <param name="resourcePath"></param>
        bool CheckHasResource(string resourcePath);

        /// <summary>
        /// 检测是否加载了资源
        /// </summary>
        bool CheckHasLoadAsset(string assetPath);

        /// <summary>
        /// 检测是否加载了ab包
        /// </summary>
        bool CheckHasLoadResouce(string resourcePath);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetPath"></param>
        void LoadAsset(string assetPath, LoadAssetCallback loadAssetCallback, object userData = null);

        /// <summary>
        /// 加载二进制资源
        /// </summary>
        void LoadBinary(string assetPath, LoadBinaryCallback loadBinaryCallback, object userData);

        /// <summary>
        /// 获取资源信息
        /// </summary>
        AssetInfo GetAssetInfo(string assetPath);

        /// <summary>
        /// 获取ab包信息
        /// </summary>
        ResourceInfo GetResouceInfo(string resourcePath);

        /// <summary>
        /// 更新所有的信息
        /// </summary>
        void UpdateAssetInfoAndResouceInfo();

        /// <summary>
        /// 增加新的已加载资源
        /// </summary>
        void AddNewAssetInfo(AssetInfo assetInfo, UnityEngine.Object @object);

        /// <summary>
        /// 增加新的已加载AB
        /// </summary>
        void AddNewResourceInfo(ResourceInfo resourceInfo, UnityEngine.AssetBundle assetBundle);

        /// <summary>
        /// 拷贝资源到Persistence地址
        /// </summary>
        /// <returns></returns>
        bool CheckAssetsNeedCopyToPersistencePath();

        /// <summary>
        /// 同步加载资源
        /// </summary>
        Task<object> LoadAssetAwait(string assetPath);
    }
}
