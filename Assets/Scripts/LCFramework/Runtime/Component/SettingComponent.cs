using Native.Construct;

namespace Native.Component
{
    public class SettingComponent : ComponentBase
    {
        public override void ManagerSet()
        {
            Manager = (IManager)LaunchComponent.Setting;
        }
    }
}
