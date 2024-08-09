using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Native.Component;

namespace Native.Resource
{
    public partial class ResourceManager
    {
        public enum AgentState
        {
            Free = 1,
            WaitToLoad = 2,
            Loading = 4,
            LoadDone = 8,
        }

        public interface IAgent<T>
        {
            AgentState AgentState { get; }
            T Info { get; }
            void Init(T info, LoadAssetCallback loadAssetCallback, object userData);

            void Init(T info);

            void Excute(float elpaseSecond);
        }

        public class LoadAssetsAgent : IAgent<AssetInfo>
        {
            private AgentState _agentState;
            private AssetInfo _assetInfo;
            private LoadAssetCallback _loadAssetCallback;
            private object _userData;
            private float _time;
            private AssetBundleRequest _request;

            public AgentState AgentState => _agentState;

            public AssetInfo Info => _assetInfo;

            public LoadAssetsAgent()
            {
                _agentState = AgentState.Free;
            }

            public void Init(AssetInfo assetInfo, LoadAssetCallback loadAssetCallback, object userData)
            {
                _agentState = AgentState.WaitToLoad;
                _assetInfo = assetInfo;
                _loadAssetCallback = loadAssetCallback;
                _userData = userData;
                _request = null;
                _time = 0;
            }

            public void Excute(float elapseSecond)
            {
                if (_agentState == AgentState.Free)
                {
                    return;
                }

                if (_agentState == AgentState.LoadDone)
                {
                    _agentState = AgentState.Free;
                    return;
                }

                if (_agentState == AgentState.WaitToLoad)
                {
                    if (Native.Component.LaunchComponent.Resource.CheckHasLoadAsset(_assetInfo.AssetPath))
                    {
                        _loadAssetCallback.LoadAssetSuccessCallback.Invoke(
                            _assetInfo.AssetPath
                            , Component.LaunchComponent.Resource.AllLoadAssets[_assetInfo]
                            , _time
                            , _userData);
                        _agentState = AgentState.LoadDone;
                        return;
                    }
                    else
                    {
                        if (Component.LaunchComponent.Resource.AllAssetAgent.Where(x => x._assetInfo == _assetInfo && x.AgentState == AgentState.Loading).Count() != 0)
                        {
                            _agentState = AgentState.Loading;
                            return;
                        }

                        if (Native.Component.LaunchComponent.Resource.CheckHasLoadResouce(_assetInfo.ResoucePath))
                        {
                            _request = Component.LaunchComponent.Resource.AllLoadResouces[Component.LaunchComponent.Resource.GetResouceInfo(_assetInfo.ResoucePath)].LoadAssetAsync(_assetInfo.AssetPath.ToLower());
                            _agentState = AgentState.Loading;
                            return;
                        }
                    }
                }

                if (_agentState == AgentState.Loading)
                {
                    if (Native.Component.LaunchComponent.Resource.CheckHasLoadAsset(_assetInfo.AssetPath))
                    {
                        _loadAssetCallback.LoadAssetSuccessCallback.Invoke(
                            _assetInfo.AssetPath
                            , Component.LaunchComponent.Resource.AllLoadAssets[_assetInfo]
                            , _time
                            , _userData);
                        _agentState = AgentState.LoadDone;
                        return;
                    }

                    if (_request != null
                        && _request.isDone)
                    {
                        Component.LaunchComponent.Resource.AddNewAssetInfo(_assetInfo, _request.asset);
                        Debug.Log($"load asset {_assetInfo.AssetPath} done!");
                        _loadAssetCallback.LoadAssetSuccessCallback.Invoke(
                            _assetInfo.AssetPath
                            , Component.LaunchComponent.Resource.AllLoadAssets[_assetInfo]
                            , _time
                            , _userData);
                        _agentState = AgentState.LoadDone;
                        return;
                    }
                }

                _time += elapseSecond;
            }

            public void Init(AssetInfo assetInfo)
            {

            }
        }

        public class LoadResourceAgent : IAgent<ResourceInfo>
        {
            private AgentState _agentState;
            private ResourceInfo _resourceInfo;
            private AssetBundleCreateRequest _request;

            public AgentState AgentState => _agentState;
            public ResourceInfo Info => _resourceInfo;

            public LoadResourceAgent()
            {
                _agentState = AgentState.Free;
            }

            public void Init(ResourceInfo resourceInfo, LoadAssetCallback loadAssetCallback, object userData)
            {

            }
            public void Init(ResourceInfo resourceInfo)
            {
                _agentState = AgentState.WaitToLoad;
                _resourceInfo = resourceInfo;
            }

            public void Excute(float elapseSecond)
            {
                if (_agentState == AgentState.Free)
                {
                    return;
                }

                if (_agentState == AgentState.LoadDone)
                {
                    _agentState = AgentState.Free;
                    return;
                }

                if (_agentState == AgentState.WaitToLoad)
                {
                    for (int i = 0; i < _resourceInfo.DependResourcePath.Length; i++)
                    {
                        if (!Native.Component.LaunchComponent.Resource.CheckHasLoadResouce(_resourceInfo.DependResourcePath[i]))
                        {
                            return;
                        }
                    }
                    _request = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(LaunchComponent.Resource.PersistenPath, _resourceInfo.ResoucePath));
                    _agentState = AgentState.Loading;
                    return;
                }

                if (_agentState == AgentState.Loading)
                {
                    if (_request.isDone)
                    {
                        Component.LaunchComponent.Resource.AddNewResourceInfo(_resourceInfo, _request.assetBundle);
                        _agentState = AgentState.LoadDone;
                        Debug.Log($"load assetbundle {_resourceInfo.ResoucePath} done!");
                        return;
                    }
                }
            }
        }

        private LoadAssetsAgent AcquireAssetAgent(AssetInfo assetInfo, LoadAssetCallback loadAssetCallback, object userData)
        {
            LoadAssetsAgent agent = null;

            foreach (var item in _allAssetAgents)
            {
                if (item.AgentState == AgentState.Free)
                {
                    agent = item;
                    break;
                }
            }

            if (agent == null)
            {
                agent = new LoadAssetsAgent();
                _allAssetAgents.Add(agent);
            }

            agent.Init(assetInfo, loadAssetCallback, userData);
            return agent;
        }

        private LoadResourceAgent AcquireResourceAgent(ResourceInfo resourceInfo)
        {
            LoadResourceAgent agent = null;

            foreach (var item in _allResourceAgents)
            {
                if (item.AgentState == AgentState.Free)
                {
                    agent = item;
                    break;
                }
            }

            if (agent == null)
            {
                agent = new LoadResourceAgent();
                _allResourceAgents.Add(agent);
            }

            agent.Init(resourceInfo);
            return agent;
        }
    }
}
