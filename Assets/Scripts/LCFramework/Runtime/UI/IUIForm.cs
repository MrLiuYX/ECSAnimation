namespace Native.UI
{
    public enum UIState
    {
        None,
        Opnening,
        Open,
        Close,
    }

    public interface IUIForm
    {
        public string AssetsPath { get; }
        public int SerializeId { get; }
        public bool InitDone { get; }
        public UIState UIState { get; }
        public UIFormLogic UIFormLogic { get; }
        public IUIGroup UIGroup { get; }
        public bool CanDestroy { get; }
        public void OnInit(object userData);
        public void OnOpen(object userData);
        public void OnCover();
        public void OnReveal();
        public void OnUpdate(float elapseSeconds);
        public void OnFixedUpdate(float elapseSeconds);
        public void OnLateUpdate(float elapseSeconds);
        public void OnCloseUpdate(float elapseSeconds);
        public void OnClose(object userData);
        public void OnDestroy();
    }
}
