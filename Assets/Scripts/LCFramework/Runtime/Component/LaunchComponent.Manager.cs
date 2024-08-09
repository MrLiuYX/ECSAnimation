using Native.Setting;
using Native.Resource;
using Native.Event;
using Native.FSM;
using Native.Procedure;
using Native.UI;
using Native.Audio;
using Native.Scheduler;

namespace Native.Component
{
    public partial class LaunchComponent
    {
        public static IResourceManager Resource;
        public static IFSMManager FSM;
        public static IEventManager Event;
        public static IProcedureManager Procedure;
        public static IUIManager UI;
        public static ISettingManager Setting;
        public static IAudioManager Audio;
        public static ISchedulerManager Scheduler;

        static LaunchComponent()
        {
            Resource = new ResourceManager();
            FSM = new FSMManager();
            Event = new EventManager();
            Procedure = new ProcedureManager();
            UI = new UIManager();
            Setting = new SettingManager();
            Audio = new AudioManager();
            Scheduler = new SchedulerManager();
            _staticClear = new System.Collections.Generic.List<System.Action>();
        }
    }
}
