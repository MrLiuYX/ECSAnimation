using Native.Component;

namespace Native.Event
{
	public class OnBakerDoneEvent : EventArgsBase
	{
		private static string _eventName = "OnBakerDoneEvent";
		private static long _eventId = 0;
		public static long EventId
		{
			get
			{
				_eventId = _eventId == 0 ? Native.Component.LaunchComponent.Event.GetEventId(_eventName) : _eventId;
				return _eventId;
			}
		}
		public override void Clear()
		{
			base.Clear();
		}
		public static OnBakerDoneEvent Create()
		{
			var data = ReferencePool.Acquire<OnBakerDoneEvent>();
			return data;
		}
	}
}
