using System;
using Native.Resource;
using Native.Component;
using System.Collections.Generic;
using UnityEngine;
using Native.Construct;

namespace Native.UI
{
    public class UIManager : ManagerBase<UIManager>, IUIManager
    {
        private sealed class InternalOpenUIFormData : IReference
        {
            public string AssetPath;
            public int SerializeId;
            public IUIForm UIForm;
            public IUIGroup UIGroup;
            public bool AllowMultiplyInstance;
            public object UserData;

            public static InternalOpenUIFormData Create(string assetPath, IUIGroup group, int serializeId, IUIForm uiform, bool allowMultiplyInstance, object userData)
            {
                var data = ReferencePool.Acquire<InternalOpenUIFormData>();
                data.AssetPath = assetPath;
                data.UIGroup = group;
                data.SerializeId = serializeId;
                data.UIForm = uiform;
                data.UserData = userData;
                data.AllowMultiplyInstance = allowMultiplyInstance;
                return data;
            }

            public void Clear()
            {
                AssetPath = default(string);
                SerializeId = default(int);
                AllowMultiplyInstance = default(bool);
                UIGroup = null;
                UIForm = null;
                UserData = null;
            }
        }

        public const string UICameraTag = "UICamera";

        public Camera UICamera { get; private set; }
        public Canvas UICanvas { get; set; }
        public Transform UIRoot { get; set; }
        public float UIFormDestroyTime { get; set; }

        private int _serializeId;
        private string[] _groupNames;
        private int[] _groupDeepLayers;
        private List<IUIGroup> _uiGroups;
        private LinkedList<IUIForm> _uiForms;
        private LoadAssetCallback _loadAssetCallback;


        private List<LinkedListNode<IUIForm>> _deleteLinkNode;
        private Action<LinkedListNode<IUIForm>, float> _updateMethod;
        private Action<LinkedListNode<IUIForm>, float> _fixedUpdateMethod;
        private Action<LinkedListNode<IUIForm>, float> _lateUpdateMethod;
        private Func<IUIForm, int, bool> _findIUIFormBySerializeId;
        private Func<IUIForm, string, bool> _findIUIFormByAssetPath;

        public override void Init()
        {
            base.Init();
            UICamera = Camera.main;
            _serializeId = 0;
            _uiGroups = new List<IUIGroup>();
            _uiForms = new LinkedList<IUIForm>();
            _loadAssetCallback = new LoadAssetCallback(OpenUISuccessCallback, OpenUIFailCallback);
            CreateUIGroup(_groupNames, _groupDeepLayers);
            _deleteLinkNode = new List<LinkedListNode<IUIForm>>();
            _updateMethod = UpdateMethod;
            _fixedUpdateMethod = FixedUpdateMethod;
            _lateUpdateMethod = LateUpdateMethod;
            _findIUIFormBySerializeId = FindIUIFormBySerializeId;
            _findIUIFormByAssetPath = FindIUIFormByAssetPath;
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnUpdate(float elapseSecond)
        {
            base.OnUpdate(elapseSecond);
            ForeachUIForm(_updateMethod, elapseSecond);

            for (int i = 0; i < _deleteLinkNode.Count; i++)
            {
                _uiForms.Remove(_deleteLinkNode[i]);
            }
            _deleteLinkNode.Clear();
        }

        public override void OnFixedUpdate(float elapseSecond)
        {
            base.OnFixedUpdate(elapseSecond);
            ForeachUIForm(_fixedUpdateMethod, elapseSecond);
        }

        public override void OnLateUpdate(float elapseSecond)
        {
            base.OnLateUpdate(elapseSecond);
            ForeachUIForm(_lateUpdateMethod, elapseSecond);
        }

        public void CloseUIForm(int serializeID, object userData = null)
        {
            IUIForm uiform = FindUIForm(FindIUIFormBySerializeId, serializeID);
            if (uiform == null)
            {
                UnityEngine.Debug.LogError($"[UIManager.CloseUIForm] Can't close uiform {serializeID}");
                return;
            }

            uiform.UIGroup.CloseUIForm(uiform, userData);
        }

        public IUIGroup GetUIGroup(string groupName)
        {
            for (int i = 0; i < _uiGroups.Count; i++)
            {
                if (_uiGroups[i].GroupName == groupName)
                    return _uiGroups[i];
            }
            return null;
        }

        public int OpenUIForm(string path, string groupName, bool allowMultiplyInstance, object userData)
        {
            if (!LaunchComponent.Resource.CheckHasAsset(path))
            {
                UnityEngine.Debug.LogError($"[UIManager.OpenUIForm] Can't find asset {path}");
                return -1;
            }

            var group = GetUIGroup(groupName);
            if (group == null)
            {
                UnityEngine.Debug.LogError($"[UIManager.OpenUIForm] Can't find UIGroup {groupName}");
                return -1;
            }

            IUIForm uiform = FindUIForm(_findIUIFormByAssetPath, path);
            if (uiform != null)
            {
                if (uiform.UIState == UIState.Open
                    && !allowMultiplyInstance)
                {
                    UnityEngine.Debug.LogError($"[UIManager.OpenUIForm] Can't open UIForm cause already open {path}");
                    return -1;
                }
            }

            var backSerializeId = -1;
            if (uiform == null
                || allowMultiplyInstance)
            {
                _serializeId++;
                backSerializeId = _serializeId;
            }
            else
            {
                backSerializeId = uiform.SerializeId;
            }

            LaunchComponent.Resource.LoadAsset(path, _loadAssetCallback, InternalOpenUIFormData.Create(path, group, backSerializeId, uiform, allowMultiplyInstance, userData));

            return backSerializeId;
        }

        public void CreateUIGroup(string[] groupNames, int[] groupDeepLayers)
        {
            for (int i = 0; i < groupNames.Length; i++)
            {
                var groupName = groupNames[i];
                var deepLayer = groupDeepLayers[i];

                var groupObj = new GameObject(groupName);
                groupObj.layer = 5;
                var rect = groupObj.AddComponent<RectTransform>();
                var canvas = groupObj.AddComponent<Canvas>();
                var group = UIGroup.Create(groupName, deepLayer, groupObj.transform);
                groupObj.transform.SetParent(UIRoot);
                groupObj.transform.localPosition = Vector3.zero;
                groupObj.transform.localRotation = Quaternion.identity;
                groupObj.transform.localScale = Vector3.one;
                groupObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                canvas.overrideSorting = true;
                canvas.sortingOrder = group.GroupDeepLayer;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector3.one * 0.5f;
                rect.sizeDelta = Vector2.zero;

                _uiGroups.Add(group);
            }
        }

        public void SetGroupData(string[] groupNames, int[] groupDeepLayers)
        {
            _groupNames = groupNames;
            _groupDeepLayers = groupDeepLayers;
        }

        private void OpenUISuccessCallback(string assetPath, object asset, float time, object userData)
        {
            var data = (InternalOpenUIFormData)userData;

            if (data.UIForm == null
                || data.AllowMultiplyInstance)
            {
                var uiInstance = UnityEngine.Object.Instantiate((GameObject)asset);
                data.UIForm = UIForm.Create(assetPath, data.SerializeId, uiInstance.GetComponent<UIFormLogic>(), data.UIGroup);
                _uiForms.AddLast(data.UIForm);
            }

            data.UIGroup.OpenUIForm(data.UIForm, data.UserData);
            ReferencePool.Release(data);
        }

        private void OpenUIFailCallback(string assetPath, float time, object userData)
        {
            UnityEngine.Debug.LogError($"[UIManager.OpenUIFailCallback] UI:{assetPath} open fail");
        }

        public IUIForm GetUIForm(int serializeId)
        {
            return FindUIForm(_findIUIFormBySerializeId, serializeId);
        }

        private IUIForm FindUIForm<T>(Func<IUIForm, T, bool> findCondition, T param)
        {
            var node = _uiForms.First;
            while (node != null)
            {
                if (findCondition.Invoke(node.Value, param))
                    return node.Value;
                node = node.Next;
            }
            return null;
        }

        private void ForeachUIForm(Action<LinkedListNode<IUIForm>, float> uiformAction, float elapseSecond)
        {
            var node = _uiForms.First;
            while (node != null)
            {
                uiformAction.Invoke(node, elapseSecond);
                node = node.Next;
            }
        }

        private void UpdateMethod(LinkedListNode<IUIForm> node, float elapseSecond)
        {
            var uiform = node.Value;
            if (uiform.UIState == UIState.Open)
            {
                uiform.OnUpdate(elapseSecond);
            }

            if (uiform.UIState == UIState.Close)
            {
                uiform.OnCloseUpdate(elapseSecond);
            }

            if (uiform.UIState == UIState.Close
                && uiform.CanDestroy)
            {
                uiform.UIGroup.DestroyUIForm(uiform);
                uiform.OnDestroy();
                _deleteLinkNode.Add(node);
            }

        }

        private void FixedUpdateMethod(LinkedListNode<IUIForm> node, float elapseSecond)
        {
            if (node.Value.UIState == UIState.Open)
                node.Value.OnFixedUpdate(elapseSecond);
        }

        private void LateUpdateMethod(LinkedListNode<IUIForm> node, float elapseSecond)
        {
            if (node.Value.UIState == UIState.Open)
                node.Value.OnFixedUpdate(elapseSecond);
        }

        private bool FindIUIFormBySerializeId(IUIForm uiform, int serializeId)
        {
            return uiform.SerializeId == serializeId;
        }

        private bool FindIUIFormByAssetPath(IUIForm uiform, string assetPath)
        {
            return uiform.AssetsPath == assetPath;
        }
    }
}
