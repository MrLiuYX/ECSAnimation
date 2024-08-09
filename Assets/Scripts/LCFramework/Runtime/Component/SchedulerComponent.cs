using Native.Construct;
using Native.Component;

namespace Native.Scheduler
{
    public class SchedulerComponent : ComponentBase
    {
        public override void ManagerSet()
        {
            Manager = (IManager)LaunchComponent.Scheduler;
        }
    }
}
