using Native.Event;
using System.Resources;

namespace Native.Construct
{
    public class ManagerBase<T> : IManager
    {

        protected int _order;

        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                bool fireEvent = _order != value;
                _order = value;
                if (fireEvent && Component.LaunchComponent.Event.InitDone)
                    Native.Component.LaunchComponent.Event.Fire(ManagerOrderChange.EventId, ManagerOrderChange.Create());
            }
        }

        public bool EnableUpdate { get; set; }
        public bool EnableFixedUpdate { get; set; }
        public bool EnableLateUpdate { get; set; }

        public virtual void Init()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnUpdate(float elapseSecond)
        {

        }

        public virtual void OnFixedUpdate(float elapseSecond)
        {

        }

        public virtual void OnLateUpdate(float elapseSecond)
        {

        }

        public virtual void ShutDown()
        {

        }

        public void InspectorDraw()
        {
#if UNITY_EDITOR
            SubInspectorDraw();
#endif
        }

        public virtual void SubInspectorDraw()
        {

        }
    }
}
