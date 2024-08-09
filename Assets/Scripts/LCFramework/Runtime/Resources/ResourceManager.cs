using Native.Event;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Native.Construct;
using System;
using LitJson;
using System.Threading.Tasks;
using System.Collections;

namespace Native.Resource
{
    public partial class ResourceManager : ManagerBase<ResourceManager>, IResourceManager
    {
        public bool CheckHasAsset(string assetPath)
        {
#if UNITY_EDITOR
            if (!IsUseEditLoadType)
            {
                return GetAssetInfo(assetPath) != null;
            }
            else
            {
                return File.Exists(assetPath);
            }
#else
            return GetAssetInfo(assetPath) != null;
#endif
        }

        public bool CheckHasResource(string resourcePath)
        {
            return GetResouceInfo(resourcePath) != null;
        }

#if UNITY_EDITOR
        private IEnumerator LocalLoad(string assetPath, LoadAssetCallback loadAssetCallback, object userData = null)
        {
            yield return new WaitForEndOfFrame();
            EditLoadAsset(assetPath, loadAssetCallback, userData);
        }
#endif

        public void LoadAsset(string assetPath, LoadAssetCallback loadAssetCallback, object userData = null)
        {
#if UNITY_EDITOR
            if (IsUseEditLoadType)
            {
                Native.Component.LaunchComponent.Instance.StartCoroutine(LocalLoad(assetPath, loadAssetCallback, userData));
                return;
            }
#endif

            if (string.IsNullOrEmpty(assetPath))
            {
                UnityEngine.Debug.LogError($"[ResourceManager.LoadAsset] AssetPath is null or empty");
                loadAssetCallback.LoadAssetFailCallback?.Invoke(assetPath, 0, userData);
                return;
            }

            if (!CheckHasAsset(assetPath))
            {
                UnityEngine.Debug.LogError($"[ResourceManager.LoadAsset] AssetPath \"{assetPath}\" is not exist");
                loadAssetCallback.LoadAssetFailCallback?.Invoke(assetPath, 0, userData);
                return;
            }

            var assetInfo = GetAssetInfo(assetPath);
            if (!CheckHasResource(assetInfo.ResoucePath))
            {
                UnityEngine.Debug.LogError($"[ResourceManager.LoadAsset] ResourcePath \"{assetInfo.ResoucePath}\" is not exist");
                loadAssetCallback.LoadAssetFailCallback?.Invoke(assetPath, 0, userData);
                return;
            }

            if (!CheckHasLoadResouce(assetInfo.ResoucePath))
            {
                LoadResouce(GetResouceInfo(assetInfo.ResoucePath));
            }

            AcquireAssetAgent(assetInfo, loadAssetCallback, userData);
        }

        public async void LoadBinary(string assetPath, LoadBinaryCallback loadBinaryCallback, object userData)
        {
            if (!CheckHasAsset(assetPath))
            {
                UnityEngine.Debug.LogError($"[ResourceManager.LoadBinary] Can't find asset {assetPath}");
                return;
            }

            var persistence = PersistenPath;

#if UNITY_EDITOR
            if (IsUseEditLoadType)
            {
                persistence = string.Empty;
            }
#endif

            if (_allLoadBinaryAssets.ContainsKey(assetPath))
            {
                loadBinaryCallback.LoadBinarySuccessCallback.Invoke(assetPath, _allLoadBinaryAssets[assetPath], 0, userData);
                return;
            }

            var time = Time.time;
            var loadPath = persistence == string.Empty ? assetPath : Path.Combine(persistence, assetPath);
            using (FileStream fs = new FileStream(loadPath, FileMode.Open))
            {
                var bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                loadBinaryCallback.LoadBinarySuccessCallback.Invoke(assetPath, bytes, Time.time - time, userData);
                if (!_allLoadBinaryAssets.ContainsKey(assetPath))
                {
                    _allLoadBinaryAssets.Add(assetPath, bytes);
                }
            }
            return;
        }

        public AssetInfo GetAssetInfo(string assetPath)
        {
            return _allAssetInfos.SingleOrDefault(x => x.AssetPath == assetPath);
        }

        public ResourceInfo GetResouceInfo(string resourcePath)
        {
            return _allResouceInfos.SingleOrDefault(x => x.ResoucePath == resourcePath);
        }

        public void UpdateAssetInfoAndResouceInfo()
        {
            ResourceHelper.LoadAsset(Path.Combine(Application.streamingAssetsPath, AssetInfoPath), (string txt, object obj) =>
            {
                var temp = JsonMapper.ToObject<List<AssetInfo>>(txt);
                for (int i = 0; i < temp.Count; i++)
                {
                    _allAssetInfos.Add(temp[i]);
                }
                Debug.Log($"load assetInfos done {txt}");
                Native.Component.LaunchComponent.Event.Fire(LoadAssetInfoTXTDone.EventId, LoadAssetInfoTXTDone.Create());
            }, null);

            ResourceHelper.LoadAsset(Path.Combine(Application.streamingAssetsPath, ResourceInfoPath), (string txt, object obj) =>
            {
                var temp = JsonMapper.ToObject<List<ResourceInfo>>(txt);
                for (int i = 0; i < temp.Count; i++)
                {
                    _allResouceInfos.Add(temp[i]);
                }
                Debug.Log($"load resourceInfos done {txt}");
                Native.Component.LaunchComponent.Event.Fire(LoadResourceInfoTXTDone.EventId, LoadResourceInfoTXTDone.Create());
            }, null);
        }

        public bool CheckHasLoadAsset(string assetPath)
        {
            return _loadAssets.Contains(assetPath);
        }

        public bool CheckHasLoadResouce(string resourcePath)
        {
            return _loadResouces.Contains(resourcePath);
        }

        public void LoadResouce(ResourceInfo resourceInfo)
        {
            for (int i = 0; i < resourceInfo.DependResourcePath.Length; i++)
            {
                if (CheckHasLoadResouce(resourceInfo.DependResourcePath[i]))
                    continue;
                var tempResourceInfo = GetResouceInfo(resourceInfo.DependResourcePath[i]);
                LoadResouce(tempResourceInfo);
            }

            if (_allResourceAgents.SingleOrDefault(x => x.Info.ResoucePath == resourceInfo.ResoucePath) == null)
            {
                AcquireResourceAgent(resourceInfo);
            }
        }

        public void AddNewAssetInfo(AssetInfo assetInfo, UnityEngine.Object @object)
        {
            _allLoadAssets.Add(assetInfo, @object);
            _loadAssets.Add(assetInfo.AssetPath);
        }

        public void AddNewResourceInfo(ResourceInfo resourceInfo, UnityEngine.AssetBundle assetBundle)
        {
            _allLoadResouces.Add(resourceInfo, assetBundle);
            _loadResouces.Add(resourceInfo.ResoucePath);
        }

        public bool CheckAssetsNeedCopyToPersistencePath()
        {
#if UNITY_EDITOR
            if (IsUseEditLoadType) return true;
#endif

            if (PersistenPath == Application.streamingAssetsPath) return true;

            var backResult = true;

            foreach (var item in _allResouceInfos)
            {
                if (!File.Exists(Path.Combine(PersistenPath, item.ResoucePath)))
                {
                    backResult = false;
                    EnqueueAssetForCopy(item.ResoucePath);
                }
            }

            foreach (var item in _allAssetInfos)
            {
                if (item.AssetMode != (int)AssetInfo.AssetModeEnum.Binary)
                    continue;
                if (!File.Exists(Path.Combine(PersistenPath, item.AssetPath)))
                {
                    backResult = false;
                    EnqueueAssetForCopy(item.AssetPath);
                }
            }

            if (!backResult)
                StartAssetCopy();

            return backResult;
        }

        private void EnqueueAssetForCopy(string path)
        {
            if (_needCopyAssetDict == null)
                _needCopyAssetDict = new List<string>();
            _needCopyAssetDict.Add(path);
            _needCopyAssetCount += 1;
        }

        private void StartAssetCopy()
        {
            foreach (var item in _needCopyAssetDict)
            {
                ResourceHelper.LoadAsset(Path.Combine(Application.streamingAssetsPath, item), LoadNeedCopyAssetDone, item);
            }
            _needCopyAssetDict.Clear();
        }

        private async void LoadNeedCopyAssetDone(Byte[] bytes, object userData)
        {
            var path = Path.Combine(PersistenPath, userData.ToString());
            var directoryPath = path.Replace($"/{Path.GetFileName(path)}", string.Empty);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                UnityEngine.Debug.Log($"Create directory {directoryPath} for copy");
            }

            if (bytes != null && userData.ToString()!="none.none")
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
#if UNITY_WEBGL
                    fs.Write(bytes, 0, bytes.Length);
#else
                    await fs.WriteAsync(bytes, 0, bytes.Length);
#endif
                    UnityEngine.Debug.Log($"Copy asset {userData.ToString()} to persistence done!");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Asset {userData.ToString()} load filed!");
            }

            _copyAssetDoneCount++;
            if (_copyAssetDoneCount == _needCopyAssetCount)
            {
                _copyAssetDoneCount = 0;
                _needCopyAssetCount = 0;
                Native.Component.LaunchComponent.Event.Fire(CopyAssetToPersistenceDone.EventId, CopyAssetToPersistenceDone.Create());
            }
        }

        public override void SubInspectorDraw()
        {
            base.SubInspectorDraw();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("当前已加载的Asset");
                for (int i = 0; i < _loadAssets.Count; i++)
                {
                    GUILayout.Label($"{i + 1}. {_loadAssets.ElementAt(i)}");
                }
                GUILayout.Label("当前正在加载的Asset");
                for (int i = 0; i < _allAssetAgents.Count; i++)
                {
                    var agent = _allAssetAgents.ElementAt(i);
                    if (_allAssetAgents.ElementAt(i).AgentState == AgentState.Loading)
                    {
                        GUILayout.Label($"{i + 1}. {agent.Info.AssetPath}");
                    }
                }
            }
            GUILayout.EndVertical();
        }

        public Task<object> LoadAssetAwait(string assetPath)
        {
            var task = new TaskCompletionSource<object>();
            LoadAsset(assetPath, new LoadAssetCallback(
                (name, asset, time, userData) =>
                {
                    task.SetResult(asset);
                }, (name, time, userData) =>
                {
                    task.SetResult(null);
                }));
            return task.Task;
        }
    }
}
