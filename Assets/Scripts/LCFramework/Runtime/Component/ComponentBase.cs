using UnityEngine;
using Native.Construct;

namespace Native.Component
{
    public abstract class ComponentBase : MonoBehaviour
    {
        [SerializeField]
        protected int _order;
        [SerializeField]
        protected bool _enableUpdate;
        [SerializeField]
        protected bool _enableFixedUpdate;
        [SerializeField]
        protected bool _enableLateUpdate;

        public IManager Manager;

        public void RegisterComponent()
        {
            LaunchComponent.Instance.RegisterComponent(this);
        }

        /// <summary>
        /// 组件上的属性设置给manager
        /// </summary>
        public virtual void ComponentSet()
        {
            Manager.Order = _order;
            Manager.EnableUpdate = _enableUpdate;
            Manager.EnableFixedUpdate = _enableFixedUpdate;
            Manager.EnableLateUpdate = _enableLateUpdate;
        }

        public virtual void GameStartExcute()
        {

        }

        /// <summary>
        /// 设置Manager
        /// </summary>
        public abstract void ManagerSet();
    }
}
