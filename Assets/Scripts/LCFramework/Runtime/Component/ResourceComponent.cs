using Native.Resource;
using UnityEngine;

namespace Native.Component
{
    public class ResourceComponent : ComponentBase
    {
        [UnityEngine.SerializeField]
        private bool IsUseEdit;

        public override void ManagerSet()
        {
            Manager = (Native.Construct.IManager)LaunchComponent.Resource;
        }

        public override void ComponentSet()
        {
            base.ComponentSet();
            ((ResourceManager)Manager).ResourceHelper = gameObject.GetComponent<ResourceHelper>();
            ((ResourceManager)Manager).IsUseEditLoadType = IsUseEdit;
            ((ResourceManager)Manager).PersistenPath = Application.platform == RuntimePlatform.WindowsPlayer ? Application.streamingAssetsPath : Application.persistentDataPath;
        }
    }
}
