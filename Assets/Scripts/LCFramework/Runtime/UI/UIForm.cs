using Native.Component;
using UnityEngine;

namespace Native.UI
{
    public class UIForm : IUIForm, IReference
    {
        public string AssetsPath { get; private set; }
        public int SerializeId { get; private set; }
        public UIFormLogic UIFormLogic { get; private set; }
        public UIState UIState { get; private set; }
        public IUIGroup UIGroup { get; private set; }
        public bool InitDone { get; private set; }
        public bool CanDestroy { get; private set; }

        private float _currentCloseTime;

        public static UIForm Create(string assetsPath, int serializeId, UIFormLogic uiFormLogic, IUIGroup uiGroup)
        {
            var data = ReferencePool.Acquire<UIForm>();
            data.AssetsPath = assetsPath;
            data.SerializeId = serializeId;
            data.UIFormLogic = uiFormLogic;
            data.InitDone = false;
            data.UIState = UIState.Opnening;
            data.UIGroup = uiGroup;
            data.CanDestroy = false;
            data.UIFormLogic.UIForm = data;
            return data;
        }

        public void Clear()
        {
            AssetsPath = default(string);
            SerializeId = default(int);
            UIFormLogic = null;
            UIGroup = null;
            InitDone = false;
            CanDestroy = false;
            UIState = UIState.None;
        }

        public void OnInit(object userData)
        {
            InitDone = true;
            UIFormLogic.OnInit(userData);
        }

        public void OnOpen(object userData)
        {
            if (!InitDone)
                OnInit(userData);
            UIState = UIState.Open;
            UIFormLogic.OnOpen(userData);
        }

        public void OnCover()
        {
            UIFormLogic.OnCover();
        }

        public void OnReveal()
        {
            UIFormLogic.OnReveal();
        }

        public void OnClose(object userData)
        {
            _currentCloseTime = 0;
            UIState = UIState.Close;
            UIFormLogic.OnClose(userData);
        }
        public void OnFixedUpdate(float elapseSeconds)
        {
            UIFormLogic.OnFixedUpdate(elapseSeconds);
        }

        public void OnUpdate(float elapseSeconds)
        {
            UIFormLogic.OnUpdate(elapseSeconds);
        }

        public void OnLateUpdate(float elapseSeconds)
        {
            UIFormLogic.OnLateUpdate(elapseSeconds);
        }

        public void OnDestroy()
        {
            UIFormLogic.Destroy();
            GameObject.DestroyImmediate(UIFormLogic.gameObject);
            ReferencePool.Release(this);
        }

        public void OnCloseUpdate(float elapseSeconds)
        {
            _currentCloseTime += elapseSeconds;
            if (_currentCloseTime >= LaunchComponent.UI.UIFormDestroyTime)
            {
                CanDestroy = true;
            }
        }
    }
}
