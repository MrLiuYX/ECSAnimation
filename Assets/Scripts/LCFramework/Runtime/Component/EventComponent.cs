using UnityEngine;
using Native.Event;

namespace Native.Component
{
    public class EventComponent : ComponentBase
    {
        public override void ManagerSet()
        {
            Manager = (Native.Construct.IManager)LaunchComponent.Event;
        }
    }
}
