using Native.Component;

namespace Native.Event
{
	public class OnOpenUISuccess : EventArgsBase
	{
		private static string _eventName = "OnOpenUISuccess";
		private static long _eventId = 0;
		public static long EventId
		{
			get
			{
				_eventId = _eventId == 0 ? Native.Component.LaunchComponent.Event.GetEventId(_eventName) : _eventId;
				return _eventId;
			}
		}

		public Native.UI.IUIForm UIForm
		{
			get;
			private set;
		}
		public override void Clear()
		{
			base.Clear();
		}
		public static OnOpenUISuccess Create(UI.IUIForm uiform)
		{
			var data = ReferencePool.Acquire<OnOpenUISuccess>();
			data.UIForm = uiform;
			return data;
		}
	}
}
