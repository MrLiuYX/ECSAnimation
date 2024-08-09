using Native.Component;
namespace Native.Event
{
    public class LoadResourceInfoTXTDone : EventArgsBase
    {
        private static string _eventName = "LoadResourceInfoTXTDone";

        private static long _eventId = 0;
        public static long EventId
        {
            get
            {
                _eventId = _eventId == 0 ? LaunchComponent.Event.GetEventId(_eventName) : _eventId;
                return _eventId;
            }
        }
        public override void Clear()
        {
            base.Clear();
        }
        public static LoadResourceInfoTXTDone Create()
        {
            var data = ReferencePool.Acquire<LoadResourceInfoTXTDone>();
            return data;
        }
    }
}
