using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Native.Resource;
using Native.Component;
using UnityEngine.Pool;

namespace Native.UI
{
    public class UIItemPool : IReference
    {
        #region 底层
        private static Dictionary<(UIFormLogic, Type), UIItemPool> _uiFormPool;
        protected static Dictionary<UIFormLogic, List<UIItemPool>> _uiFormUpdate;
        private const string UIItemPath = "Assets/AssetBundleRes/Main/Prefabs/UIItems/{0}.prefab";

        static UIItemPool()
        {
            _uiFormPool = new Dictionary<(UIFormLogic, Type), UIItemPool>();
            _uiFormUpdate = new Dictionary<UIFormLogic, List<UIItemPool>>();
        }

        public static UIItemPool AcquirePool<T>(UIFormLogic uiForm) where T : UIItemLogic
        {
            UIItemPool data = null;

            if (!_uiFormPool.ContainsKey((uiForm, typeof(T))))
            {
                _uiFormPool.Add((uiForm, typeof(T)), UIItemPool.Create(typeof(T).Name));
            }
            data = _uiFormPool[(uiForm, typeof(T))];

            if (!_uiFormUpdate.ContainsKey(uiForm))
            {
                _uiFormUpdate.Add(uiForm, ListPool<UIItemPool>.Get());
            }

            if (!_uiFormUpdate[uiForm].Contains(data))
            {
                _uiFormUpdate[uiForm].Add(data);
            }

            return data;
        }

        public static void ReleaseSignle(UIFormLogic uiForm, UIItemLogic logic)
        {
            if (!_uiFormPool.ContainsKey((uiForm, logic.GetType()))) return;

            var pool = _uiFormPool[(uiForm, logic.GetType())];
            pool.Unspawn(logic);
        }

        public static void ReleasePool<T>(UIFormLogic uiForm)
        {
            if (!_uiFormPool.ContainsKey((uiForm, typeof(T)))) return;

            var pool = _uiFormPool[(uiForm, typeof(T))];
            pool.UnspawnAll();
        }

        public static void ReleaseAllPool(UIFormLogic uiForm)
        {
            foreach (var key in _uiFormPool.Keys)
            {
                if (key.Item1 == uiForm)
                {
                    _uiFormPool[key].UnspawnAll();
                }
            }
        }

        public static void DestroyAllPool(UIFormLogic uiForm)
        {
            var removeKeys = ListPool<(UIFormLogic, Type)>.Get();
            foreach (var key in _uiFormPool.Keys)
            {
                if (key.Item1 == uiForm)
                {
                    _uiFormPool[key].UnspawnAll(true);
                    removeKeys.Add(key);
                }
            }

            for (int i = 0; i < removeKeys.Count; i++)
            {
                _uiFormPool.Remove(removeKeys[i]);
            }

            if (_uiFormUpdate.ContainsKey(uiForm))
            {
                var pool = _uiFormUpdate[uiForm];
                _uiFormUpdate.Remove(uiForm);
                ListPool<UIItemPool>.Release(pool);
            }

            ListPool<(UIFormLogic, Type)>.Release(removeKeys);
        }

        public static void OnUpdate(UIFormLogic uiForm, float elapseSeconds)
        {
            if (!_uiFormUpdate.ContainsKey(uiForm)) return;

            var pools = _uiFormUpdate[uiForm];
            for (int i = 0; i < pools.Count; i++)
            {
                pools[i].OnUpdate(elapseSeconds);
            }
        }

        private void OnUpdate(float elapseSeconds)
        {
            for (int i = 0; i < _useUIItemLogic.Count; i++)
            {
                _useUIItemLogic[i].OnUpdate(elapseSeconds);
            }
        }

        public static void OnFixedUpdate(UIFormLogic uiForm, float elapseSeconds)
        {
            if (!_uiFormUpdate.ContainsKey(uiForm)) return;

            var pools = _uiFormUpdate[uiForm];
            for (int i = 0; i < pools.Count; i++)
            {
                pools[i].OnFixedUpdate(elapseSeconds);
            }
        }

        private void OnFixedUpdate(float elapseSeconds)
        {
            for (int i = 0; i < _useUIItemLogic.Count; i++)
            {
                _useUIItemLogic[i].OnFixedUpdate(elapseSeconds);
            }
        }

        public static void OnLateUpdate(UIFormLogic uiForm, float elapseSeconds)
        {
            if (!_uiFormUpdate.ContainsKey(uiForm)) return;

            var pools = _uiFormUpdate[uiForm];
            for (int i = 0; i < pools.Count; i++)
            {
                pools[i].OnLateUpdate(elapseSeconds);
            }
        }

        private void OnLateUpdate(float elapseSeconds)
        {
            for (int i = 0; i < _useUIItemLogic.Count; i++)
            {
                _useUIItemLogic[i].OnLateUpdate(elapseSeconds);
            }
        }
        #endregion

        #region 池子逻辑
        private Queue<UIItemLogic> _freeUIItemLogic;
        private List<UIItemLogic> _useUIItemLogic;

        private Queue<UIFormLogic> _callbackUIForm;
        private Queue<LoadAssetCallback> _callback;
        private Queue<object> _callbackUserData;

        private LoadAssetCallback _internalCallback;
        private string _uiitemLogicPath;

        public void Clear()
        {
            _freeUIItemLogic.Clear();
            _useUIItemLogic.Clear();
            _callbackUIForm.Clear();
            _callback.Clear();
            _callbackUserData.Clear();
            _internalCallback = null;
            _uiitemLogicPath = string.Empty;
        }

        public static UIItemPool Create(string name)
        {
            var data = ReferencePool.Acquire<UIItemPool>();

            data._freeUIItemLogic = data._freeUIItemLogic ?? new Queue<UIItemLogic>();
            data._useUIItemLogic = data._useUIItemLogic ?? new List<UIItemLogic>();

            data._callback = data._callback ?? new Queue<LoadAssetCallback>();
            data._callbackUIForm = data._callbackUIForm ?? new Queue<UIFormLogic>();
            data._callbackUserData = data._callbackUserData ?? new Queue<object>();
            data._internalCallback = new LoadAssetCallback(data.LoadCallbackSuccess, data.LoadCallbackFailed);
            data._uiitemLogicPath = string.Format(UIItemPath, name);
            return data;
        }

        public void Spawn(UIFormLogic uiForm, LoadAssetCallback callback, object userData)
        {
            _callback.Enqueue(callback);
            _callbackUIForm.Enqueue(uiForm);
            _callbackUserData.Enqueue(userData);
            InternalSpawn();
        }

        public void Unspawn<T>(T logic) where T : UIItemLogic
        {
            logic.OnUnsapwn();
            logic.gameObject.SetActive(false);
            logic.transform.SetParent(LaunchComponent.UI.UIRoot);
            _freeUIItemLogic.Enqueue(logic);
            _useUIItemLogic.Remove(logic);
        }

        public void UnspawnAll(bool destroy = false)
        {
            for (int i = 0; i < _useUIItemLogic.Count; i++)
            {
                var logic = _useUIItemLogic[i];
                logic.OnUnsapwn();
                logic.gameObject.SetActive(false);
                logic.transform.SetParent(LaunchComponent.UI.UIRoot);
                _freeUIItemLogic.Enqueue(logic);
            }
            _useUIItemLogic.Clear();

            if (destroy)
            {
                DestroyPool();
            }
        }

        private void DestroyPool()
        {
            while (_freeUIItemLogic.Count != 0)
            {
                var logic = _freeUIItemLogic.Dequeue();
                GameObject.Destroy(logic.gameObject);
            }
            ReferencePool.Release(this);
        }

        private void InternalSpawn()
        {
            if (_freeUIItemLogic.Count != 0)
            {
                var uiform = _callbackUIForm.Dequeue();
                var userData = _callbackUserData.Dequeue();
                var callback = _callback.Dequeue();

                var logic = _freeUIItemLogic.Dequeue();
                _useUIItemLogic.Add(logic);
                logic.gameObject.SetActive(true);
                logic.OnSpawn(uiform, userData);
                callback?.LoadAssetSuccessCallback?.Invoke(_uiitemLogicPath, logic, 0, userData);
            }
            else
            {
                LaunchComponent.Resource.LoadAsset(_uiitemLogicPath, _internalCallback);
            }
        }

        private void LoadCallbackSuccess(string name, object asset, float time, object userData)
        {
            var uiform = _callbackUIForm.Dequeue();
            var callbackUserData = _callbackUserData.Dequeue();
            var callback = _callback.Dequeue();

            var logic = GameObject.Instantiate((GameObject)asset).GetComponent<UIItemLogic>();

            if (logic == null)
            {
                Debug.LogError($"[UIItemPool] 未在{name}资源上找到UIItemnLogic脚本");
                return;
            }
            _useUIItemLogic.Add(logic);

            logic.OnInitBind();
            logic.OnSpawn(uiform, callbackUserData);
            callback?.LoadAssetSuccessCallback?.Invoke(_uiitemLogicPath, logic, 0, callbackUserData);
        }

        private void LoadCallbackFailed(string name, float time, object userData)
        {
            Debug.LogError($"[UIItemPool] Can't load asset {name}");
        }
        #endregion
    }
}
