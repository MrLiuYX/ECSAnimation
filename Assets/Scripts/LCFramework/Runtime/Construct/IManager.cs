namespace Native.Construct
{
    public interface IManager
    {
        int Order { get; set; }
        bool EnableUpdate { get; set; }
        bool EnableFixedUpdate { get; set; }
        bool EnableLateUpdate { get; set; }

        void Init();

        void OnEnable();

        void OnUpdate(float elapseSecond);

        void OnFixedUpdate(float elapseSecond);

        void OnLateUpdate(float elapseSecond);

        void OnDisable();

        void ShutDown();

        void InspectorDraw();
    }
}
