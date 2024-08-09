using Native.FSM;

namespace Native.Component
{
    public class FSMComponent : ComponentBase
    {
        public override void ManagerSet()
        {
            Manager = (Native.Construct.IManager)LaunchComponent.FSM;
        }
    }
}
